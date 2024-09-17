

namespace DtoLayer.Dtos.CommentDtos
{
    public class CreateCommentWithMentionsDto
    {
        public int PostId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public List<string> Mentions { get; set; } // Etiketlenecek kullanıcı adları
    }
}
