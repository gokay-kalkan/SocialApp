

namespace EntityLayer.Entities
{
    public class FollowRequest
    {
        public int FollowRequestId { get; set; }
        public string RequesterId { get; set; }  // Takip isteyen kullanıcı
        public string TargetUserId { get; set; }  // Takip edilmek istenen kullanıcı
        public bool IsApproved { get; set; }  // Takip isteğinin durumu
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        public virtual User Requester { get; set; }
        public virtual User TargetUser { get; set; }
        public virtual ICollection<FollowRequestNotification> FollowRequestNotifications { get; set; }
    }
}
