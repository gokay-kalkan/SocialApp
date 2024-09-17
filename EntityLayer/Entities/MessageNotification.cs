

namespace EntityLayer.Entities
{
    public class MessageNotification
    {
        public int MessageNotificationId { get; set; }

        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }

        public string UserId { get; set; }
        public bool IsRead { get; set; }

        public virtual User User { get; set; }
    }
}
