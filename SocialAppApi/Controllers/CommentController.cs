using BusinessLayer.Abstract;
using DtoLayer.Dtos.ApiResponseDto;
using DtoLayer.Dtos.CommentDtos;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SocialAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly FollowerService _followerService;
        private readonly CommentService _commentService;
        private readonly NotificationService _notificationService;
        private readonly CommentLikeService _commentLikeService;
        private readonly PostService _postService;

        public CommentController(UserManager<User> userManager, FollowerService followerService, CommentService commentService, NotificationService notificationService, CommentLikeService commentLikeService, PostService postService)
        {
            _userManager = userManager;
            _followerService = followerService;
            _commentService = commentService;
            _notificationService = notificationService;
            _commentLikeService = commentLikeService;
            _postService = postService;
        }


        [HttpPost("addComment")]
        public IActionResult AddComment(CreateCommentDto model)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            // Yorumu yapılacak gönderiyi buluyoruz
            var post = _postService.GetById(model.PostId);
            if (post == null)
            {
                return NotFound(new ApiResponse<string>(false, "Gönderi bulunamadı.", null));
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

            // Yorum oluşturuyoruz
            var comment = new Comment
            {
                PostId = model.PostId,
                UserId = currentUserId,
                Content = model.Content,
                CreatedDate = DateTime.UtcNow
            };

            _commentService.Add(comment);


            var notification = new Notification
            {
                UserId = post.UserId,
                PostId = post.PostId,
                Type = $"{User.Identity.Name} senin postuna yorum yaptı",
                CreatedDate = DateTime.UtcNow,
                IsRead = false
            };

            _notificationService.Add(notification);

            return Ok(new ApiResponse<string>(true, "Yorum başarıyla eklendi.", null));
        }

        [HttpPost("replyToComment/{parentCommentId}")]
        public IActionResult ReplyToComment(int parentCommentId, [FromBody] ReplyToCommentDto model)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            // Yanıt verilecek yorumu buluyoruz
            var parentComment = _commentService.GetById(parentCommentId);
            if (parentComment == null)
            {
                return NotFound(new ApiResponse<string>(false, "Yanıt verilecek yorum bulunamadı.", null));
            }

            // Gönderiyi yapan kullanıcıyı buluyoruz
            var post = _postService.GetById(parentComment.PostId);
            if (post == null)
            {
                return NotFound(new ApiResponse<string>(false, "Gönderi bulunamadı.", null));
            }

            // Following kontrolü: Yorum sahibi ile kullanıcı arasında takip ilişkisi var mı?
            var isFollowing = _followerService.List(f =>
                (f.FollowerUserId == currentUserId && f.FollowingUserId == parentComment.UserId) ||
                (f.FollowerUserId == parentComment.UserId && f.FollowingUserId == currentUserId)
            ).Any();

            if (!isFollowing)
            {
                return Forbid("Bu yorumu yanıtlama yetkiniz yok.");
            }

            // Yanıt olarak yeni bir yorum oluşturuyoruz
            var replyComment = new Comment
            {
                PostId = parentComment.PostId,
                UserId = currentUserId,
                Content = model.Content,
                CreatedDate = DateTime.UtcNow,
                ParentCommentId = parentCommentId // Parametreden alınan ParentCommentId
            };

            _commentService.Add(replyComment);

            // Bildirim oluşturma: Yanıt verilen yorumu yazan kişiye bildirim gönderilir
            var notification = new Notification
            {
                UserId = parentComment.UserId,
                PostId = parentComment.PostId,
                Type = $"{User.Identity.Name} senin yorumuna yanıt verdi",
                CreatedDate = DateTime.UtcNow,
                IsRead = false
            };

            _notificationService.Add(notification);

            return Ok(new ApiResponse<string>(true, "Yorum başarıyla yanıtlandı.", null));
        }

        [HttpPost("addCommentWithMentions")]
        public IActionResult AddCommentWithMentions(CreateCommentWithMentionsDto model)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            var post = _postService.GetById(model.PostId);
            if (post == null)
            {
                return NotFound(new ApiResponse<string>(false, "Gönderi bulunamadı.", null));
            }

            var isFollowing = _followerService.List(f =>
              (f.FollowerUserId == currentUserId && f.FollowingUserId == model.UserId) ||
              (f.FollowerUserId == model.UserId && f.FollowingUserId == currentUserId)
          ).Any();

            if (!isFollowing)
            {
                return Forbid("Bu yorumu yanıtlama yetkiniz yok.");
            }

            var comment = new Comment
            {
                PostId = model.PostId,
                UserId = currentUserId,
                Content = model.Content,
                CreatedDate = DateTime.UtcNow
            };

            _commentService.Add(comment);

            // Opsiyonel Mention
            if (model.Mentions != null && model.Mentions.Any())
            {
                var mentionedUsers = _userManager.Users
                    .Where(u => model.Mentions.Contains(u.UserName))
                    .ToList();

                foreach (var mentionedUser in mentionedUsers)
                {
                    var notification = new Notification
                    {
                        UserId = mentionedUser.Id,
                        PostId = comment.PostId,
                        Type = $"{User.Identity.Name} sizi bir yorumda etiketledi",
                        CreatedDate = DateTime.UtcNow,
                        IsRead = false
                    };

                    _notificationService.Add(notification);
                }
            }

            var postNotification = new Notification
            {
                UserId = post.UserId,
                PostId = post.PostId,
                Type = $"{User.Identity.Name} senin postuna yorum yaptı",
                CreatedDate = DateTime.UtcNow,
                IsRead = false
            };

            _notificationService.Add(postNotification);

            return Ok(new ApiResponse<string>(true, "Yorum başarıyla eklendi.", null));
        }


        [HttpDelete("deleteComment")]
        public IActionResult DeleteComment([FromQuery]int commentId)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            // Yorumu buluyoruz
            var comment = _commentService.GetById(commentId);
            if (comment == null)
            {
                return NotFound(new ApiResponse<string>(false, "Yorum bulunamadı.", null));
            }

            // Yorumu yapılan gönderiyi buluyoruz
            var post = _postService.GetById(comment.PostId);
            if (post == null)
            {
                return NotFound(new ApiResponse<string>(false, "Gönderi bulunamadı.", null));
            }

            // Yorumun sahibini veya postun sahibini kontrol ediyoruz
            if (comment.UserId != currentUserId && post.UserId != currentUserId)
            {
                return Forbid("Bu yorumu silme yetkiniz yok.");
            }

            _commentService.Delete(comment);

            return Ok(new ApiResponse<string>(true, "Yorum başarıyla silindi.", null));
        }

        [HttpPut("editComment")]
        public async Task<IActionResult> EditComment([FromQuery]int commentId, [FromBody] UpdateCommentDto model)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            // Yorumu buluyoruz
            var comment = _commentService.GetById(commentId);
            if (comment == null)
            {
                return NotFound(new ApiResponse<string>(false, "Yorum bulunamadı.", null));
            }

            // Yorumun sahibini kontrol ediyoruz
            if (comment.UserId != currentUserId)
            {
                return Forbid("Bu yorumu düzenleme yetkiniz yok.");
            }

            // Yorumu güncellenmesi için hazırlıyoruz
            
            comment.UserId = currentUserId;
            comment.Content = model.Content;
            comment.CreatedDate = DateTime.UtcNow;

            _commentService.Update(comment);

            return Ok(new ApiResponse<string>(true, "Yorum başarıyla güncellendi.", null));
        }

        [HttpPost("likeComment")]
        public IActionResult LikeComment([FromForm] LikeCommentDto model)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            // Yorumu buluyoruz
            var comment = _commentService.GetById(model.CommentId);
            if (comment == null)
            {
                return NotFound(new ApiResponse<string>(false, "Yorum bulunamadı.", null));
            }

            // Yorumu yapılan gönderiyi buluyoruz
            var post = _postService.GetById(comment.PostId);
            if (post == null)
            {
                return NotFound(new ApiResponse<string>(false, "Gönderi bulunamadı.", null));
            }

            if (post.UserId != currentUserId)
            {
                // Eğer post başka bir kullanıcıya aitse, takip kontrolü yapılır
                var isFollowing = _followerService.List(f =>
                    f.FollowerUserId == currentUserId && f.FollowingUserId == post.UserId).Any();

                if (!isFollowing)
                {
                    return Forbid("Bu gönderiyi beğenme yetkiniz yok.");
                }
            }


            // Zaten beğenip beğenmediğini kontrol et
            var existingLike = _commentLikeService.List(l=> l.UserId == currentUserId && l.CommentId == model.CommentId).FirstOrDefault();
            if (existingLike != null)
            {
                return BadRequest(new ApiResponse<string>(false, "Bu yorumu zaten beğendiniz.", null));
            }

            // Yorum beğenme işlemi
            var like = new CommentLike
            {

                CommentId = model.CommentId,
                UserId = currentUserId,
                CreatedDate = DateTime.UtcNow
            };

            _commentLikeService.Add(like);

            // Bildirim oluşturma
            var notification = new Notification
            {
                UserId = comment.UserId,
                PostId = comment.PostId,
                Type = $"{User.Identity.Name} senin yorumunu beğendi",
                CreatedDate = DateTime.UtcNow,
                IsRead = false
            };

            _notificationService.Add(notification);

            return Ok(new ApiResponse<string>(true, "Yorum başarıyla beğenildi.", null));
        }


        [HttpGet("getCommentsForUserPosts")]
        public async Task<IActionResult> GetCommentsForUserPosts()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            var userPosts = _postService.List(p => p.UserId == currentUserId).ToList();

            // Ardından, bu gönderilere yapılmış tüm yorumları alıyoruz
            var postIds = userPosts.Select(p => p.PostId).ToList();

            var comments = (from c in _commentService.List()
                            join p in userPosts on c.PostId equals p.PostId
                            select c).ToList();

            var commentDtos = comments.Select(c => new GetCommentsForUserDto
            {
                CommentId = c.CommentId,
                PostId = c.PostId,
                Content = c.Content,
                CreatedDate = c.CreatedDate,
                UserName = c.User.UserName,
                UserId=c.UserId
            }).ToList();

            return Ok(new ApiResponse<List<GetCommentsForUserDto>>(true, "Gönderilerinize yapılan yorumlar listelendi", commentDtos));
        }


        [HttpGet("getCommentsForFollowedUsersPosts")]
        public async Task<IActionResult> GetCommentsForFollowedUsersPosts()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            // Kullanıcının takip ettiği kullanıcıları al
            var followedUsers = _followerService.List(f => f.FollowerUserId == currentUserId).Select(f => f.FollowingUserId).ToList();

            // Takip edilen kullanıcıların gönderilerini al
            var followedUsersPosts = (from post in _postService.List()
                                      join follower in followedUsers on post.UserId equals follower
                                      select post).ToList();

            // Bu gönderilere yapılmış tüm yorumları al
            var postIds = followedUsersPosts.Select(p => p.PostId).ToList();

            var comments = (from comment in _commentService.List()
                            join post in followedUsersPosts on comment.PostId equals post.PostId
                            select comment).ToList();

            var commentDtos = comments.Select(c => new GetCommentsForUserDto
            {
                CommentId = c.CommentId,
                PostId = c.PostId,
                Content = c.Content,
                CreatedDate = c.CreatedDate,
                UserName = c.User.UserName,
                UserId=c.UserId
                
            }).ToList();

            return Ok(new ApiResponse<List<GetCommentsForUserDto>>(true, "Takip ettiğiniz kullanıcıların gönderilerine yapılan yorumlar listelendi", commentDtos));
        }


    }
}
