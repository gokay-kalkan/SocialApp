

namespace DtoLayer.Dtos.FollowRequestNotificationDtos
{
    public class FollowRequestNotificationListDto
    {
        public int FollowRequestNotificationId { get; set; }
        public string Type { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; } // Mevcut olan alan
        public int FollowRequestId { get; set; } // Eklenen alan

        public string RequesterId { get; set; }
        public string TargetUserId { get; set; }

        public string Status { get; set; } // Pending, Approved, Rejected gibi durumlar
        public bool IsFollowedBack { get; set; } // Geri takip işlemi yapıldı mı
        public bool IsApproved { get; set; } // Takip Onaylandı mı
        public bool IsFollowBackPending { get; set; } // Geri takip işlemi durumu
    }

}
