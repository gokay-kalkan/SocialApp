

namespace DtoLayer.Dtos.PostDtos
{
    public class FollowedUsersPostListDto
    {
        public int PostId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; }  // Post sahibinin kullanıcı adı
        public string UserId { get; set; }  
        public int LikesCount { get; set; }  // Posta yapılan beğeni sayısı
        public int CommentsCount { get; set; }  // Posta yapılan yorum sayısı
        public string UserProfilePicture { get; set; }  // Post sahibinin profil resmi URL'si (isteğe bağlı)
        public string ImageUrl { get; set; }  // Postun içerdiği resim URL'si (isteğe bağlı)
        public bool IsLikedByCurrentUser { get; set; }  // Giriş yapmış kullanıcının bu postu beğenip beğenmediği
    }
}
