using BusinessLayer.Abstract;
using DtoLayer.Dtos.ApiResponseDto;
using DtoLayer.Dtos.CommentDtos;
using DtoLayer.Dtos.PostDtos;
using DtoLayer.Dtos.UserDtos;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace SocialAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly PostService _postService;
        private readonly PostLikeService _postLikeService;
        private readonly NotificationService _notificationService;
        private readonly FollowerService _followerService;
        private readonly IHostingEnvironment _env;
        public PostController(UserManager<User> userManager, PostService postService, IHostingEnvironment env, PostLikeService postLikeService, NotificationService notificationService, FollowerService followerService)
        {
            _userManager = userManager;
            _postService = postService;
            _env = env;
            _postLikeService = postLikeService;
            _notificationService = notificationService;
            _followerService = followerService;
        }

        [HttpPost("createPost")]

        public async Task<IActionResult> CreatePost([FromForm] CreatePostDto createPostDto)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            //if (createPostDto.Media == null && string.IsNullOrEmpty(createPostDto.MediaUrl))
            //{
            //    return BadRequest(new ApiResponse<string>(false, "Lütfen bir dosya yükleyin veya bir URL girin.", null));
            //}

            var post = new Post
            {
                UserId = currentUserId,
                Content = createPostDto.Content,
                CreatedDate = DateTime.UtcNow,
                Status=true
            };

            // Dosya yükleme işlemi
            if (createPostDto.Media != null && createPostDto.Media.Length > 0)
            {
                var path = Path.Combine(_env.WebRootPath, "uploads/posts");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path = Path.Combine(path, createPostDto.Media.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await createPostDto.Media.CopyToAsync(stream);
                }
                var returnUrl = $"{Request.Scheme}://{Request.Host}/uploads/posts/{createPostDto.Media.FileName}";
                post.MediaUrl = returnUrl;
            }
            else if (!string.IsNullOrEmpty(createPostDto.MediaUrl))
            {
                post.MediaUrl = createPostDto.MediaUrl; // Harici URL'yi kullan
            }

            _postService.Add(post);
            return Ok(new ApiResponse<string>(true, "Gönderi başarıyla oluşturuldu.", post.PostId.ToString()));
        }


        [HttpDelete("deletePost")]
        public IActionResult DeletePost([FromQuery]int postId)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            var post = _postService.GetById(postId);
            if (post == null)
            {
                return NotFound(new ApiResponse<string>(false, "Gönderi bulunamadı.", null));
            }


            if (post.UserId != currentUserId)
            {
                return Forbid("Bu gönderiyi silme yetkiniz yok.");
            }

            post.Status = false;

            _postService.Update(post);

            return Ok(new ApiResponse<string>(true, "Gönderi başarıyla silindi.", null));
        }

        [HttpPut("updatePost")]
        public async Task<IActionResult> UpdatePost([FromQuery]int postId, [FromForm] UpdatePostDto updatePostDto)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }


            var post = _postService.GetById(postId);
            if (post == null)
            {
                return NotFound(new ApiResponse<string>(false, "Gönderi bulunamadı.", null));
            }


            if (post.UserId != currentUserId)
            {
                return Forbid("Bu Gönderiyi Güncelleme Yetkiniz Yok");
            }


            post.Content = updatePostDto.Content ?? post.Content;

            // Yeni bir medya dosyası var mı kontrol ediyoruz
            if (updatePostDto.Media != null && updatePostDto.Media.Length > 0)
            {
                // Eski dosyayı silme işlemi (isteğe bağlı)
                if (!string.IsNullOrEmpty(post.MediaUrl) && !post.MediaUrl.StartsWith("http"))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, "uploads/posts", post.MediaUrl);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // Yeni dosyayı yükleyip güncelliyoruz
                var path = Path.Combine(_env.WebRootPath, "uploads/posts");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path = Path.Combine(path, updatePostDto.Media.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await updatePostDto.Media.CopyToAsync(stream);
                }
                var returnUrl = $"{Request.Scheme}://{Request.Host}/uploads/posts/{updatePostDto.Media.FileName}";
                post.MediaUrl = returnUrl; // Medya dosyasının yeni yolu
            }
            else if (!string.IsNullOrEmpty(updatePostDto.MediaUrl))
            {
                // Yeni bir URL girilmişse, bu URL'yi güncelliyoruz
                post.MediaUrl = updatePostDto.MediaUrl;
            }
           
            _postService.Update(post);
            return Ok(new ApiResponse<string>(true, "Gönderi başarıyla güncellendi.", null));
        }



        [HttpGet("getUserPosts")]
        public IActionResult GetUserPosts()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            // Kullanıcının kendi gönderilerini alıyoruz ve gerekli bilgileri ekliyoruz
            var posts = _postService.List(p => p.UserId == currentUserId && p.Status==true)
                .Select(p => new PostListDto
                {
                    PostId = p.PostId,
                    UserId=p.UserId,
                    Content = p.Content,
                    ImageUrl = p.MediaUrl,
                    CreatedDate = p.CreatedDate,
                    LikesCount = p.PostLikes.Count, // Toplam beğeni sayısı
                    CommentsCount = p.Comments.Count, // Toplam yorum sayısı
                    IsLikedByCurrentUser = p.PostLikes.Any(l => l.UserId == currentUserId), // Kullanıcının kendi gönderisini beğenip beğenmediği

                }).ToList();

            // Eğer gönderi bulunmuyorsa
            if (posts == null || !posts.Any())
            {
                return NotFound(new ApiResponse<List<PostListDto>>(false, "Gönderi bulunamadı.", new List<PostListDto>()));
            }

            return Ok(new ApiResponse<List<PostListDto>>(true, "Kullanıcının gönderileri başarıyla listelendi.", posts));
        }



        [HttpPost("likePost")]
        public async Task<IActionResult> LikePost(int postId)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            // Postu alıyoruz
            var post = _postService.GetById(postId);
            if (post == null)
            {
                return NotFound(new ApiResponse<string>(false, "Post bulunamadı.", null));
            }
            if (post.UserId != currentUserId)
            {
                var isFollowing = _followerService.List(f =>
                    f.FollowerUserId == currentUserId &&
                    f.FollowingUserId == post.UserId).Any();

                if (!isFollowing)
                {
                    return Forbid("Bu gönderiye yorum yapma yetkiniz yok.");
                }
            }


            var existingLike = _postLikeService.List(l => l.PostId == postId && l.UserId == currentUserId).FirstOrDefault();

            if (existingLike != null)
            {
                // Beğeni varsa, kaldır
                _postLikeService.Delete(existingLike);
                return Ok(new ApiResponse<string>(true, "Beğeni kaldırıldı.", null));
            }

            else
            {
                // Beğeni yoksa, ekle
                var like = new PostLike
                {
                    PostId = postId,
                    UserId = currentUserId,
                    CreatedDate = DateTime.UtcNow
                };
                _postLikeService.Add(like);

                // Bildirimi oluştur
                var notification = new Notification
                {
                    UserId = post.UserId,
                    Type = $"{User.Identity.Name} senin postunu beğendi",
                    PostId = postId,
                    CreatedDate = DateTime.UtcNow,
                    IsRead = false
                };

                _notificationService.Add(notification);

                return Ok(new ApiResponse<string>(true, "Post başarıyla beğenildi ve bildirim gönderildi.", null));
            }
        }


        [HttpGet("getFollowedUsersPosts")]
        public async Task<IActionResult> GetFollowedUsersPosts()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            // Kullanıcının takip ettiği kişilerin kullanıcı ID'lerini alıyoruz
            var followedUserIds = _followerService.List(f => f.FollowerUserId == currentUserId)
                                                  .Select(f => f.FollowingUserId)
                                                  .ToList();

            if (!followedUserIds.Any())
            {
                return Ok(new ApiResponse<List<FollowedUsersPostListDto>>(true, "Takip edilen kullanıcı yok.", new List<FollowedUsersPostListDto>()));
            }

            // Takip edilen kullanıcıların postlarını alıyoruz
            var posts = (from post in _postService.List(x=>x.Status==true)
                         where followedUserIds.Contains(post.UserId)
                         orderby post.CreatedDate descending
                         select new FollowedUsersPostListDto
                         {
                             PostId = post.PostId,
                             Content = post.Content,
                             CreatedDate = post.CreatedDate,
                             UserName = post.User.UserName,
                             UserId=post.UserId,
                             LikesCount = post.PostLikes.Count,
                             CommentsCount = post.Comments.Count,
                             UserProfilePicture = post.User.ProfilePicture,
                             ImageUrl = post.MediaUrl,
                             IsLikedByCurrentUser = post.PostLikes.Any(l => l.UserId == currentUserId)
                         }).ToList();
            return Ok(new ApiResponse<List<FollowedUsersPostListDto>>(true, "Takip edilen kullanıcıların postları başarıyla getirildi.", posts));
        }


      
        [HttpGet("getUserPostsIfFollowed")]
        public async Task<IActionResult> GetUserPostsIfFollowed(string targetUserId)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            // Kullanıcının, gidilen profildeki kişiyi takip edip etmediğini kontrol ediyoruz
            var isFollowing = _followerService.List(f => f.FollowerUserId == currentUserId && f.FollowingUserId == targetUserId).Any();
            if (!isFollowing && currentUserId != targetUserId)
            {
                return Unauthorized(new ApiResponse<string>(false, "Bu kullanıcının gönderilerini görmek için takip etmelisiniz.", null));
            }

            // Gidilen kullanıcının gönderilerini alıyoruz
            var posts = _postService.List(p => p.UserId == targetUserId && p.Status==true)
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new FollowedUsersPostListDto
                {
                    PostId = p.PostId,
                    Content = p.Content,
                    CreatedDate = p.CreatedDate,
                    UserName = p.User.UserName,
                    UserId=p.UserId,
                    LikesCount = p.PostLikes.Count,
                    CommentsCount = p.Comments.Count,
                    UserProfilePicture = p.User.ProfilePicture,
                    ImageUrl = p.MediaUrl,
                    IsLikedByCurrentUser = p.PostLikes.Any(l => l.UserId == currentUserId)
                }).ToList();

            return Ok(new ApiResponse<List<FollowedUsersPostListDto>>(true, "Kullanıcının gönderileri başarıyla getirildi.", posts));
        }

        [HttpGet("getPostLikes")]
        public IActionResult GetPostLikes([FromQuery] int postId)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            // Gönderiye ait beğenileri çekiyoruz
            var postLikes = _postLikeService.List(pl => pl.PostId == postId)
                .Select(pl => new PostLikeDto
                {
                    UserId = pl.UserId,
                    UserName = pl.User.UserName,
                    ProfilePicture = pl.User.ProfilePicture,
                    CreatedDate = pl.CreatedDate
                }).ToList();

            if (!postLikes.Any())
            {
                return NotFound(new ApiResponse<List<PostLikeDto>>(false, "Bu gönderiye beğeni yapılmamış.", new List<PostLikeDto>()));
            }

            return Ok(new ApiResponse<List<PostLikeDto>>(true, "Gönderiyi beğenen kullanıcılar başarıyla listelendi.", postLikes));
        }

    }
}
