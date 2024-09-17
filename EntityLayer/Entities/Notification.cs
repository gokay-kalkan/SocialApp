

namespace EntityLayer.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public int? PostId { get; set; }
       
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual User User { get; set; }  // Navigation Property
        public virtual Post Post { get; set; }  // Navigation Property
      
    }
}
