using BusinessLayer.Abstract;
using DtoLayer.Dtos.ApiResponseDto;
using DtoLayer.Dtos.MessageNotificationDtos;
using DtoLayer.Dtos.NotificationDtos;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SocialAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageNotificationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        private readonly MessageNotificationService _messageNotificationService;

        public MessageNotificationController(MessageNotificationService messageNotificationService, UserManager<User> userManager)
        {
            _messageNotificationService = messageNotificationService;
            _userManager = userManager;
        }

        [HttpGet("getUserMessageAllNotifications")]
        public async Task<IActionResult> GetUserMessageAllNotifications()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            // Sisteme giren kullanıcıya ait bildirimleri getiriyoruz
            var notifications = _messageNotificationService.List(n => n.UserId == currentUserId && !n.IsRead).ToList();

            var notificationDtos = notifications.Select(c => new MessageNotificationListDto
            {

                CreatedDate = c.CreatedDate,
                Content = c.Content,
                MessageNotificationId = c.MessageNotificationId,
            

            }).ToList();


           
            return Ok(new ApiResponse<List<MessageNotificationListDto>>(true, "Bildirimler başarıyla getirildi.", notificationDtos));
        }
    }
}
