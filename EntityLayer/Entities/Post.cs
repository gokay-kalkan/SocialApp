
namespace EntityLayer.Entities
{
    public class Post
    {
        public int PostId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public string? MediaUrl { get; set; }
        public DateTime CreatedDate { get; set; }

        public bool Status { get; set; }

        public virtual User User { get; set; }  // Navigation Property
        public virtual ICollection<Comment> Comments { get; set; }  // Navigation Property
        public virtual ICollection<PostLike> PostLikes { get; set; }  // Navigation Property
        public virtual ICollection<Notification> Notifications { get; set; }  // Navigation Property
    }
}
