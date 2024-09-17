

namespace DtoLayer.Dtos.PostDtos
{
    public class PostListDto
    {
        public int PostId { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; } // Kullanıcının adı
        public string UserId { get; set; } // Kullanıcının adı

        public int LikesCount { get; set; }  // Posta yapılan beğeni sayısı
        public int CommentsCount { get; set; }  // Posta yapılan yorum sayısı

        public bool IsLikedByCurrentUser { get; set; }  // Giriş yapmış kullanıcının bu postu beğenip beğenmediği

        public int MyProperty { get; set; }
    }
}
