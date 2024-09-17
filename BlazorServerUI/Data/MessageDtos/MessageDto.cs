namespace BlazorServerUI.Data.MessageDtos
{
    public class MessageDto
    {
        public int MessageId { get; set; } // Mesajın ID'si
        public string Content { get; set; } // Mesajın içeriği
        public DateTime CreatedDate { get; set; } // Mesajın gönderildiği tarih
        public DateTime? UpdatedDate { get; set; } // Mesajın düzenlendiği tarih
        public string SenderId { get; set; } // Mesajı gönderen kullanıcının ID'si
        public string ReceiverId { get; set; } // Mesajı alan kullanıcının ID'si
        public string MediaUrl { get; set; }
        public bool IsRead { get; set; } // Mesajın okunup okunmadığı durumu
        public string SenderName { get; set; } // Mesajın okunup okunmadığı durumu
    }
}
