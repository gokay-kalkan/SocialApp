using Azure.Core;
using BusinessLayer.Abstract;
using DtoLayer.Dtos.ApiResponseDto;
using DtoLayer.Dtos.FollowerDtos;
using DtoLayer.Dtos.FollowRequestDtos;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SocialAppApi.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class FollowerController : ControllerBase
    {
        private readonly FollowerService _followerService;
        private readonly FollowRequestService _followRequestService;
        private readonly FollowRequestNotificationService _followRequestNotificationService;

        private readonly UserManager<User> _userManager;
        public FollowerController(FollowerService followerService, UserManager<User> userManager, FollowRequestService followRequestService, FollowRequestNotificationService followRequestNotificationService)
        {
            _followerService = followerService;
            _userManager = userManager;
            _followRequestService = followRequestService;
            _followRequestNotificationService = followRequestNotificationService;
        }
        // takip isteği atma
        [HttpPost("followUser")]
        public async Task<IActionResult> FollowUser([FromBody] CreateFollowerDto followUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(false, "Invalid data", null));
            }

            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            // Takip edilecek kullanıcıyı alıyoruz
            var followingUser = await _userManager.FindByIdAsync(followUserDto.FollowingUserId);
            if (followingUser == null)
            {
                return NotFound(new ApiResponse<string>(false, "Takip edilecek kullanıcı bulunamadı.", null));
            }

            var requesterUser = await _userManager.FindByIdAsync(currentUserId);
            if (requesterUser == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Takip isteği gönderen kullanıcı bulunamadı.", null));
            }

            var existingRequest = _followRequestService.GetByPredicate(fr =>
                fr.RequesterId == currentUserId &&
                fr.TargetUserId == followUserDto.FollowingUserId &&
                !fr.IsApproved);

            if (existingRequest != null)
            {
                return BadRequest(new ApiResponse<string>(false, "Bu kullanıcıya zaten bir takip isteği gönderdiniz.", null));
            }

            var followRequest = new FollowRequest
            {
                RequesterId = currentUserId,
                TargetUserId = followUserDto.FollowingUserId,
                IsApproved = false
            };

            _followRequestService.Add(followRequest);

            // Bildirim hedef kullanıcıya gönderilmeli
            var followRequestNotification = new FollowRequestNotification
            {
                UserId = followUserDto.FollowingUserId, // Takip edilecek kullanıcıya bildirim gönderiliyor
                Type = $"{requesterUser.UserName} size Takip İsteği Gönderdi",
                CreatedDate = DateTime.UtcNow,
                IsRead = false,
                FollowRequestId = followRequest.FollowRequestId
            };

            _followRequestNotificationService.Add(followRequestNotification);

            return Ok(new ApiResponse<string>(true, "Takip isteği gönderildi.", null));
        }

        //takip isteğini onaylama
        [HttpPost("approvefollowrequest")]
        public async Task<IActionResult> ApproveFollowRequest([FromQuery] int requestId)
        {
            var followRequest = _followRequestService.GetById(requestId);
            if (followRequest == null)
            {
                return NotFound(new ApiResponse<string>(false, "Takip isteği bulunamadı.", null));
            }

            var currentUserId = _userManager.GetUserId(User);
            if (followRequest.TargetUserId != currentUserId)
            {
                return Unauthorized(new ApiResponse<string>(false, "Bu takip isteğini onaylama yetkiniz yok.", null));
            }

            followRequest.IsApproved = true;
            _followRequestService.Update(followRequest);

            var follower = new Follower
            {
                FollowerUserId = followRequest.RequesterId,
                FollowingUserId = followRequest.TargetUserId,
                FollowerUser = await _userManager.FindByIdAsync(followRequest.RequesterId),
                FollowingUser = await _userManager.FindByIdAsync(followRequest.TargetUserId)
            };

            _followerService.Add(follower);

            var requestApprovingUser = await _userManager.FindByIdAsync(currentUserId);

            var followRequestNotification = new FollowRequestNotification
            {
                UserId = followRequest.RequesterId, // Takip isteğini gönderen kişiye bildirim gider
                Type = $"{requestApprovingUser.UserName} Takip İsteğinizi Onayladı",
                CreatedDate = DateTime.UtcNow,
                IsRead = false,
                FollowRequestId = followRequest.FollowRequestId
            };

            _followRequestNotificationService.Add(followRequestNotification);



            return Ok(new ApiResponse<string>(true, "Takip isteği onaylandı.", null));
        }

        //takip isteğini reddetme
        [HttpPost("rejectfollowrequest")]
        public async Task<IActionResult> RejectFollowRequest([FromQuery] int requestId)
        {
            // Takip isteğini buluyoruz
            var followRequest = _followRequestService.GetById(requestId);
            if (followRequest == null)
            {
                return NotFound(new ApiResponse<string>(false, "Takip isteği bulunamadı.", null));
            }

            // Oturum açmış kullanıcının ID'sini alıyoruz
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            // Takip isteği, sadece hedef kullanıcı tarafından reddedilebilir
            if (followRequest.TargetUserId != currentUserId)
            {
                return Unauthorized(new ApiResponse<string>(false, "Bu takip isteğini reddetme yetkiniz yok.", null));
            }

            // Takip isteğini silme işlemi
            _followRequestService.Delete(followRequest);

            return Ok(new ApiResponse<string>(true, "Takip isteği reddedildi.", null));
        }

        //takip isteğini onayladıktna sonra geri takip etme isteği gönderme
        [HttpPost("followback")]
        public async Task<IActionResult> FollowBack([FromQuery] string followerId)
        {
            try
            {
                var currentUserId = _userManager.GetUserId(User);
                if (currentUserId == null)
                {
                    return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
                }


                var currentUser = await _userManager.FindByIdAsync(currentUserId);
                if (currentUser == null)
                {
                    return Unauthorized(new ApiResponse<string>(false, "Geçerli kullanıcı bulunamadı.", null));
                }

                var followerUser = await _userManager.FindByIdAsync(followerId);
                if (followerUser == null)
                {
                    return NotFound(new ApiResponse<string>(false, "Takip edilecek kullanıcı bulunamadı.", null));
                }

                // Zaten takip ilişkisini kontrol et
                var existingFollowByCurrentUser = _followerService.GetByPredicate(f =>
                    f.FollowerUserId == currentUserId && f.FollowingUserId == followerId);

                if (existingFollowByCurrentUser != null)
                {
                    return BadRequest(new ApiResponse<string>(false, "Bu kullanıcıyı zaten takip ediyorsunuz.", null));
                }

                // Kullanıcı sizi takip ediyor mu kontrol et
                var existingFollowByFollower = _followerService.GetByPredicate(f =>
                    f.FollowerUserId == followerId && f.FollowingUserId == currentUserId);

                if (existingFollowByFollower == null)
                {
                    return BadRequest(new ApiResponse<string>(false, "Bu kullanıcı zaten sizi takip etmiyor.", null));
                }

                // İlgili takip isteğini çekin
                var relatedFollowRequest = _followRequestService.GetByPredicate(fr =>
                    fr.RequesterId == followerId && fr.TargetUserId == currentUserId && fr.IsApproved);

                if (relatedFollowRequest == null)
                {
                    return NotFound(new ApiResponse<string>(false, "İlgili takip isteği bulunamadı.", null));
                }

                // Geri takip bildirimi oluşturulurken doğru kullanıcı bilgilerini set edin.
                var followBackNotification = new FollowRequestNotification
                {
                    UserId = relatedFollowRequest.RequesterId, // Bildirimi alacak kullanıcı, yani target user (örneğin Murat)
                    Type = $"{relatedFollowRequest.TargetUser.UserName} sizi geri takip etmek istiyor", // Bildirimi gönderen kişi yani requester (örneğin Gökay)
                    CreatedDate = DateTime.UtcNow,
                    IsRead = false,
                    FollowRequestId = relatedFollowRequest.FollowRequestId // İlgili FollowRequestId'yi set edin
                };

                _followRequestNotificationService.Add(followBackNotification);
                return Ok(new ApiResponse<string>(true, "Geri takip isteği gönderildi.", null));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return StatusCode(500, new ApiResponse<string>(false, $"Sunucu hatası: {ex.Message}", null));
            }
        }



        [HttpPost("approveFollowBack")]
        public async Task<IActionResult> ApproveFollowBack([FromQuery] string followerId, [FromQuery] int followRequestId)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            // İlgili FollowRequest kaydını alıyoruz
            var followRequest = _followRequestService.GetById(followRequestId);
            if (followRequest == null || followRequest.RequesterId != followerId || followRequest.TargetUserId == currentUserId)
            {
                return BadRequest(new ApiResponse<string>(false, "Geçersiz takip isteği.", null));
            }


            string requesterId = followRequest.RequesterId;
            string targetUserId = followRequest.TargetUserId;

            var relatedFollowRequest = _followRequestService.GetByPredicate(fr =>
                    fr.RequesterId == followerId && fr.IsApproved && fr.TargetUserId == targetUserId);
            //var existingFollowRelation = _followerService.GetByPredicate(f =>
            //    f.FollowerUserId != requesterId && f.FollowingUserId != targetUserId);

            //if (existingFollowRelation != null)
            //{
            //    return BadRequest(new ApiResponse<string>(false, "Bu kullanıcıyı zaten takip ediyorsunuz.", null));
            //}

            var mutualFollow = new Follower
            {
                FollowerUserId = targetUserId,
                FollowingUserId = requesterId,
                FollowerUser = await _userManager.FindByIdAsync(targetUserId),
                FollowingUser = await _userManager.FindByIdAsync(requesterId)
            };

            _followerService.Add(mutualFollow);

            var followBackNotification = new FollowRequestNotification
            {
                UserId = targetUserId,
                Type = $"{relatedFollowRequest.Requester.UserName} Geri takip isteğinizi onayladı", // Bildirimi gönderen kişi yani requester (örneğin Gökay)
                CreatedDate = DateTime.UtcNow,
                IsRead = false,
                FollowRequestId = relatedFollowRequest.FollowRequestId // İlgili FollowRequestId'yi set edin
            };

            _followRequestNotificationService.Add(followBackNotification);
            return Ok(new ApiResponse<string>(true, "Geri takip isteği onaylandı.", null));
        }






        // Geri takip isteğini reddetme
        [HttpPost("rejectFollowBack")]
        public async Task<IActionResult> RejectFollowBack([FromQuery] string followerId)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            // Geri takip isteğini reddettiğinize dair bir işlem yapabilirsiniz (bildirim vs.)
            return Ok(new ApiResponse<string>(true, "Geri takip isteği reddedildi.", null));
        }


        // kullanıcıyı sonradan takipten çıkarma
        [HttpPost("unfollowUser")]
        public async Task<IActionResult> UnfollowUser([FromQuery] string followingUserId)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            var followRelation = _followerService.GetByPredicate(f =>
                f.FollowerUserId == currentUserId &&
                f.FollowingUserId == followingUserId);

            if (followRelation == null)
            {
                return NotFound(new ApiResponse<string>(false, "Bu kullanıcıyı zaten takip etmiyorsunuz.", null));
            }

            _followerService.Delete(followRelation);
            return Ok(new ApiResponse<string>(true, "Kullanıcı takipten çıkarıldı.", null));
        }


        //gelen takip isteklerini listele
        [HttpGet("followrequests")]
        public async Task<IActionResult> GetFollowRequests()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            var followRequests = _followRequestService.List(fr =>
                fr.TargetUserId == currentUserId && !fr.IsApproved);

            var followRequestDtos = followRequests.Select(fr => new FollowRequestDto
            {
                RequestId = fr.FollowRequestId,
                RequesterName = fr.Requester.UserName,
                RequestDate = fr.RequestDate
            }).ToList();

            return Ok(new ApiResponse<List<FollowRequestDto>>(true, "Takip istekleri listelendi.", followRequestDtos));
        }




        [HttpGet("checkfollowing")]
        public async Task<IActionResult> CheckFollowing(string currentUserId, string targetUserId)
        {
            // Mevcut kullanıcının hedef kullanıcıyı takip edip etmediğini kontrol et
            var followRelation = _followerService.GetByPredicate(f =>
                f.FollowerUserId == currentUserId &&
                f.FollowingUserId == targetUserId);

            if (followRelation != null)
            {
                return Ok(new ApiResponse<bool>(true, "Kullanıcı takip ediyor.", true));
            }

            return Ok(new ApiResponse<bool>(true, "Kullanıcı takip etmiyor.", false));
        }

        [HttpGet("isFollowRequestSent")]
        public async Task<IActionResult> IsFollowRequestSent(string currentUserId, string targetUserId)
        {
            var followRequest = _followRequestService.GetByPredicate(fr =>
                fr.RequesterId == currentUserId &&
                fr.TargetUserId == targetUserId &&
                !fr.IsApproved);

            if (followRequest != null)
            {
                return Ok(new ApiResponse<bool>(true, "Takip isteği gönderilmiş.", true));
            }

            return Ok(new ApiResponse<bool>(true, "Takip isteği gönderilmemiş.", false));
        }
    }
}
