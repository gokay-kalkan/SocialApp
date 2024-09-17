using BlazorServerUI.Data.CommentDtos;

namespace BlazorServerUI.Data.PostDtos
{
    public class UnifiedPostDto
    {
        public int PostId { get; set; }
        public int CommentId { get; set; }
        
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; } // Kullanıcının adı
        public string UserId { get; set; } // Kullanıcının adı
        public bool ShowCommentSection { get; set; } = false; // Yorum bölümü aç/kapat kontrolü

        public bool IsLikedByCurrentUser { get; set; }
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>(); // Postun yorumları

        public int LikesCount { get; set; }  // Posta yapılan beğeni sayısı
        public int CommentsCount { get; set; }

        public string CommentContent { get; set; }
    }
}
