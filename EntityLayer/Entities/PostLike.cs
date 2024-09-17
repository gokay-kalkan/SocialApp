

namespace EntityLayer.Entities
{
    public class PostLike
    {
        public int PostLikeId { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Post Post { get; set; }  // Navigation Property
        public virtual User User { get; set; }  // Navigation Property
    }
}
