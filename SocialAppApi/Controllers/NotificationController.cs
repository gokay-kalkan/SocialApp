using BusinessLayer.Abstract;
using DtoLayer.Dtos.ApiResponseDto;
using DtoLayer.Dtos.FollowRequestNotificationDtos;
using DtoLayer.Dtos.NotificationDtos;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SocialAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly NotificationService _notificationService;
        private readonly FollowRequestNotificationService _followRequestNotificationService;

        public NotificationController(UserManager<User> userManager, NotificationService notificationService, FollowRequestNotificationService followRequestNotificationService)
        {
            _userManager = userManager;
            _notificationService = notificationService;
            _followRequestNotificationService = followRequestNotificationService;
        }

        [HttpGet("getNotificationCount")]
        public async Task<IActionResult> GetNotificationCount()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<int>(false, "Kullanıcı giriş yapmamış.", 0));
            }

            // Takip isteklerinin sayısını hesapla
            var followRequestCount = _followRequestNotificationService.List(n => n.UserId == currentUserId && !n.IsRead).Count;

            // Beğeni ve yorum bildirimlerinin sayısını hesapla
            var notificationCount = _notificationService.List(n => n.UserId == currentUserId && !n.IsRead).Count;

            // Toplam bildirim sayısını döndür
            var totalNotifications = followRequestCount + notificationCount;

            return Ok(new ApiResponse<int>(true, "Bildirim sayısı başarıyla getirildi.", totalNotifications));
        }

        [HttpGet("getUserAllNotifications")]
        public async Task<IActionResult> GetUserAllNotifications()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            // Sisteme giren kullanıcıya ait bildirimleri getiriyoruz
            var notifications = _notificationService.List(n => n.UserId == currentUserId && !n.IsRead).ToList();

            var notificationDtos = notifications.Select(c => new NotificationListDto
            {
               
                CreatedDate = c.CreatedDate,
                Type = c.Type,
                NotificationId = c.NotificationId,
                PostId=c.PostId

            }).ToList();


            foreach (var notification in notifications)
            {
                notification.IsRead = true;  // Bildirim görüldü olarak işaretlenir
                _notificationService.Update(notification);
            }
      
            return Ok(new ApiResponse<List<NotificationListDto>>(true, "Bildirimler başarıyla getirildi.", notificationDtos));
        }
    }
}
