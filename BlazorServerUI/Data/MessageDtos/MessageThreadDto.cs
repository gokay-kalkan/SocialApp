namespace BlazorServerUI.Data.MessageDtos
{
    public class MessageThreadDto
    {
        public string ThreadId { get; set; } // Mesajlaştığın kişinin ID'si (Kullanıcının ID'si)
        public string ParticipantName { get; set; } // Mesajlaştığın kişinin kullanıcı adı
        public string ParticipantProfilePicture { get; set; } // Mesajlaştığın kişinin profil resmi URL'si
        public string LastMessage { get; set; } // En son gönderilen mesajın içeriği
        public DateTime LastMessageDate { get; set; } // En son mesajın gönderildiği tarih
        public bool IsRead { get; set; } // Son mesajın okunup okunmadığını gösterir

        // Mesajların listesi
        public List<MessageDto> Messages { get; set; } = new List<MessageDto>();
    }
}
