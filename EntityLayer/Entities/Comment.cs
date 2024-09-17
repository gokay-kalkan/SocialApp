
namespace EntityLayer.Entities
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
       
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }

        public int? ParentCommentId { get; set; }  // Yanıtlanan yorumun ID'si, nullable olabilir
        public virtual Comment ParentComment { get; set; }  // Navigation Property
        public virtual ICollection<Comment> Replies { get; set; }  
        public virtual Post Post { get; set; }  // Navigation Property
        public virtual User User { get; set; }

        public virtual ICollection<CommentLike>CommentLikes { get; set; }// Navigation Property

    }
}
