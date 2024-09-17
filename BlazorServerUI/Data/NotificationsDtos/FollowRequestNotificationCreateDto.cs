namespace BlazorServerUI.Data.NotificationsDtos
{
    public class FollowRequestNotificationCreateDto
    {
        public string UserId { get; set; }
        public string Type { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
        public int FollowRequestId { get; set; }
    }
}
