namespace BlazorServerUI.Data.MessageDtos
{
    public class CreateMessageDto
    {
        public string ReceiverId { get; set; }
        public string Content { get; set; }

        public IFormFile? Media { get; set; } // Dosya yükleme seçeneği
        public string? MediaUrl { get; set; }  // Harici URL seçeneği
    }
}
