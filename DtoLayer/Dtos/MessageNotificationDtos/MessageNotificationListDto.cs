

namespace DtoLayer.Dtos.MessageNotificationDtos
{
    public class MessageNotificationListDto
    {
        public int MessageNotificationId { get; set; }
        public string Content { get; set; }
        public int? PostId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
