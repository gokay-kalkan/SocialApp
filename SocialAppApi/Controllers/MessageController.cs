using BusinessLayer.Abstract;
using DtoLayer.Dtos.ApiResponseDto;
using DtoLayer.Dtos.MessageDto;
using DtoLayer.Dtos.MessageNotificationDtos;
using DtoLayer.Dtos.NotificationDtos;
using DtoLayer.Dtos.PostDtos;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace SocialAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly MessageService _messageService;
        private readonly MessageNotificationService _messageNotificationService;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;

        public MessageController(UserManager<User> userManager, MessageService messageService, MessageNotificationService messgaeNotificationService, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            _userManager = userManager;
            _messageService = messageService;
            _messageNotificationService = messgaeNotificationService;
            _env = env;
        }

        [HttpPost("sendMessage")]
        public async Task<IActionResult> SendMessage([FromForm] CreateMessageDto createMessageDto)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            var receiverUser = await _userManager.FindByIdAsync(createMessageDto.ReceiverId);
            if (receiverUser == null)
            {
                return NotFound(new ApiResponse<string>(false, "Alıcı kullanıcı bulunamadı.", null));
            }

            var message = new Message
            {
                SenderId = currentUserId,
                ReceiverId = createMessageDto.ReceiverId,
                Content = createMessageDto.Content,
                CreatedDate = DateTime.UtcNow,
                IsRead = false
            };

            // Eğer medya yüklenmişse dosya işlemi yapıyoruz
            if (createMessageDto.Media != null && createMessageDto.Media.Length > 0)
            {
                var path = Path.Combine(_env.WebRootPath, "uploads/messages",createMessageDto.Media.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await createMessageDto.Media.CopyToAsync(stream);
                }
                var returnUrl = $"{Request.Scheme}://{Request.Host}/uploads/messages/{createMessageDto.Media.FileName}";
                message.MediaUrl = returnUrl;
            }

            else if (!string.IsNullOrEmpty(createMessageDto.MediaUrl))
            {
                message.MediaUrl = createMessageDto.MediaUrl; // Harici URL'yi kullan
            }

            _messageService.Add(message);

            //burada bildirim gönder ayrı tablo aç mesaj bildirimleri diye

            MessageNotification messageNotification = new MessageNotification()
            {
                UserId = createMessageDto.ReceiverId,
                Content = $"{User.Identity.Name} size bir mesaj gönderdi.",
                CreatedDate = DateTime.Now,
                IsRead = false,

            };

            _messageNotificationService.Add(messageNotification);

            return Ok(new ApiResponse<string>(true, "Mesaj başarıyla gönderildi.", null));
        }

        [HttpGet("getUserMessages")]
        public async Task<IActionResult> GetUserMessages([FromQuery]string receiverId)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            var messages = _messageService.List(m =>
                (m.SenderId == currentUserId && m.ReceiverId == receiverId) ||
                (m.SenderId == receiverId && m.ReceiverId == currentUserId)).ToList();

            var messageDtos = messages.Select(m => new MessageListDto
            {
                MessageId = m.MessageId,
                Content = m.Content,
                CreatedDate = m.CreatedDate,
                IsRead = m.IsRead,
                MediaUrl = m.MediaUrl,
                SenderName = m.Sender.UserName,
                SenderId=m.SenderId,
                ReceiverId=m.ReceiverId,
                ReceiverName = m.Receiver.UserName
            }).ToList();

            var messageNotifications = _messageNotificationService.List(n => n.UserId == currentUserId && !n.IsRead).ToList();

            var notificationDtos = messageNotifications.Select(c => new MessageNotificationListDto
            {

                CreatedDate = c.CreatedDate,
                Content = c.Content,
                MessageNotificationId = c.MessageNotificationId,
          

            }).ToList();

            foreach (var notification in messageNotifications)
            {
                notification.IsRead = true;  // Bildirim görüldü olarak işaretlenir
                _messageNotificationService.Update(notification);
            }

            return Ok(new ApiResponse<List<MessageListDto>>(true, "Mesajlar başarıyla getirildi.", messageDtos));
        }

        [HttpPut("markAsRead")]
        public IActionResult MarkAsRead([FromQuery]int messageId)
        {
            var message = _messageService.GetById(messageId);
            if (message == null)
            {
                return NotFound(new ApiResponse<string>(false, "Mesaj bulunamadı.", null));
            }

            message.IsRead = true;
            _messageService.Update(message);

            return Ok(new ApiResponse<string>(true, "Mesaj başarıyla okundu olarak işaretlendi.", null));
        }

        [HttpDelete("deleteMessage")]
        public async Task<IActionResult> DeleteMessage([FromQuery]int messageId)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            var message = _messageService.GetById(messageId);
            if (message == null)
            {
                return NotFound(new ApiResponse<string>(false, "Mesaj bulunamadı.", null));
            }

            // Mesajın göndericisi veya alıcısı mı kontrol et
            if (message.SenderId != currentUserId && message.ReceiverId != currentUserId)
            {
                return Forbid("Bu mesajı silme yetkiniz yok.");
            }

            // Mesajı silme işlemi
            _messageService.Delete(message);

            return Ok(new ApiResponse<string>(true, "Mesaj başarıyla silindi.", null));
        }

        [HttpGet("getUserMessageThreads")]
        public async Task<IActionResult> GetUserMessageThreads()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            // Kullanıcının mesajlaştığı kişileri bul
            var messageThreads = _messageService.List(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
                .GroupBy(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
                .Select(g => new MessageThreadDto
                {
                    ThreadId = g.Key,
                    LastMessage = g.OrderByDescending(m => m.CreatedDate).FirstOrDefault().Content,

                    LastMessageDate = g.OrderByDescending(m => m.CreatedDate).FirstOrDefault().CreatedDate,

                    ParticipantName = g.FirstOrDefault(m => m.SenderId == g.Key || m.ReceiverId == g.Key)
                        .SenderId == g.Key ? g.FirstOrDefault(m => m.SenderId == g.Key).Sender.UserName
                                           : g.FirstOrDefault(m => m.ReceiverId == g.Key).Receiver.UserName,

                    ParticipantProfilePicture = g.FirstOrDefault(m => m.SenderId == g.Key || m.ReceiverId == g.Key)
                        .SenderId == g.Key ? g.FirstOrDefault(m => m.SenderId == g.Key).Sender.ProfilePicture
                                           : g.FirstOrDefault(m => m.ReceiverId == g.Key).Receiver.ProfilePicture
                })
                .ToList();

            return Ok(new ApiResponse<List<MessageThreadDto>>(true, "Mesaj oturumları başarıyla getirildi.", messageThreads));
        }

        [HttpPut("updateMessage")]
        public async Task<IActionResult> UpdateMessage([FromQuery]int messageId, [FromBody] UpdateMessageDto updateMessageDto)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            var message = _messageService.GetById(messageId);
            if (message == null)
            {
                return NotFound(new ApiResponse<string>(false, "Mesaj bulunamadı.", null));
            }

            // Mesajın sahibi mi kontrol et
            if (message.SenderId != currentUserId)
            {
                return Forbid("Bu mesajı düzenleme yetkiniz yok.");
            }

            // Mesaj içeriğini güncelle
            message.Content = updateMessageDto.Content;
            message.UpdatedDate = DateTime.UtcNow; // Güncellenme tarihini ayarla
            _messageService.Update(message);

            return Ok(new ApiResponse<string>(true, "Mesaj başarıyla güncellendi.", null));
        }

        [HttpGet("getUnreadMessageCount")]
        public async Task<IActionResult> GetUnreadMessageCount()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<int>(false, "Kullanıcı giriş yapmamış.", 0));
            }

            // Okunmamış mesajları filtrele
            var unreadMessageCount = _messageService
         .List(m => m.ReceiverId == currentUserId && !m.IsRead)
         .GroupBy(m => m.SenderId)
         .Count();

            return Ok(new ApiResponse<int>(true, "Okunmamış mesaj sayısı başarıyla getirildi.", unreadMessageCount));
        }

    }


}
