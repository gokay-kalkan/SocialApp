namespace BlazorServerUI.Data.NotificationsDtos
{
    public class NotificationListDto
    {
        public int NotificationId { get; set; }
        public string Type { get; set; }
        public int? PostId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
