using BusinessLayer.Abstract;
using DtoLayer.Dtos.ApiResponseDto;
using DtoLayer.Dtos.CommentDtos;
using DtoLayer.Dtos.FollowRequestNotificationDtos;
using DtoLayer.Dtos.NotificationDtos;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace SocialAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowRequestNotificationController : ControllerBase
    {
      
        private readonly FollowRequestNotificationService _followRequestNotificationService;
        private readonly UserManager<User> _userManager;

        public FollowRequestNotificationController( UserManager<User> userManager, FollowRequestNotificationService followRequestNotificationService)
        {
            
            _userManager = userManager;
            _followRequestNotificationService = followRequestNotificationService;
        }

        [HttpGet("getUserFollowRequestNotifications")]
        public async Task<IActionResult> GetUserFollowRequestNotifications()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            // Bildirimleri alıyoruz
            var notifications = _followRequestNotificationService.List(n => n.UserId == currentUserId && !n.IsRead).ToList();

            // Bildirimleri DTO'ya çeviriyoruz ve gerekli bilgileri ekliyoruz
            var notificationDtos = notifications.Select(c => new FollowRequestNotificationListDto
            {
                FollowRequestNotificationId = c.FollowRequestNotificationId,
                CreatedDate = c.CreatedDate,
                Type = c.Type,
                FollowRequestId = c.FollowRequestId,
                RequesterId = c.FollowRequest.RequesterId, // Takip isteğini gönderen kullanıcı
                TargetUserId = c.FollowRequest.TargetUserId, // Takip isteğinin hedef kullanıcısı
                UserId = c.UserId, // Bildirimi alan kullanıcı
                IsApproved = c.Type.Contains("Takip İsteğinizi Onayladı"),
                IsFollowedBack = c.Type.Contains("Geri Takip Onaylandı"),
                IsFollowBackPending = c.Type.Contains("sizi geri takip etmek istiyor")
            }).ToList();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                _followRequestNotificationService.Update(notification);
            }

            return Ok(new ApiResponse<List<FollowRequestNotificationListDto>>(true, "Bildirimler başarıyla getirildi.", notificationDtos));
        }






    }
}
