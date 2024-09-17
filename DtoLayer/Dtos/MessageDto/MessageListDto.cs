

namespace DtoLayer.Dtos.MessageDto
{
    public class MessageListDto
    {
        public int MessageId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; } // Mesajın düzenlendiği tarih
        public bool IsRead { get; set; }

        public string MediaUrl { get; set; }
        public string SenderName { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string ReceiverName { get; set; }
    }
}
