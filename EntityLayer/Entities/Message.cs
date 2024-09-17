

namespace EntityLayer.Entities
{
    public class Message
    {
        public int MessageId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsRead { get; set; }

        public string? MediaUrl { get; set; }
        public virtual User Sender { get; set; }  // Navigation Property
        public virtual User Receiver { get; set; }  // Navigation Property
    }
}
