

namespace EntityLayer.Entities
{
    public class FollowRequestNotification
    {
        public int FollowRequestNotificationId { get; set; }
        public string Type { get; set; }
        public DateTime CreatedDate { get; set; }

        public string UserId { get; set; }
        public bool IsRead { get; set; }
        public int FollowRequestId { get; set; }

        public virtual FollowRequest FollowRequest { get; set; }
        public virtual User User { get; set; }
    }
}
