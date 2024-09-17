using BlazorServerUI.Data.CommentDtos;

namespace BlazorServerUI.Data.PostDtos
{
    public class FollowedUsersPostListDto
    {
        public int PostId { get; set; }
        public string Content { get; set; }

        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; }  // Post sahibinin kullanıcı adı
        public int LikesCount { get; set; }  // Posta yapılan beğeni sayısı
        public int CommentsCount { get; set; }  // Posta yapılan yorum sayısı
        public string UserProfilePicture { get; set; }  // Post sahibinin profil resmi URL'si
        public string ImageUrl { get; set; }  // Postun içerdiği resim URL'si
        public bool IsLikedByCurrentUser { get; set; }  // Giriş yapmış kullanıcının bu postu beğenip beğenmediği
        public bool ShowCommentSection { get; set; } = false; // Yorum bölümü aç/kapat kontrolü
        public string NewCommentContent { get; set; } = ""; // Yeni yorum içeriği
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>(); // Postun yorumları
    }

}
