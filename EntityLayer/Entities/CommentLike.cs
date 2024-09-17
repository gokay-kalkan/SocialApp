

namespace EntityLayer.Entities
{
    public class CommentLike
    {
        public int CommentLikeId { get; set; }
        public int CommentId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Comment Comment { get; set; }  // Navigation Property
        public virtual User User { get; set; }  // Navigation Property
    }
}
