

namespace DtoLayer.Dtos.NotificationDtos
{
    public class NotificationListDto
    {
        public int NotificationId { get; set; }
        public string Type { get; set; }
        public int? PostId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
