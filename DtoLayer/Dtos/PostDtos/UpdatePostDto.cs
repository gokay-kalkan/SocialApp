using Microsoft.AspNetCore.Http;


namespace DtoLayer.Dtos.PostDtos
{
    public class UpdatePostDto
    {
        public string Content { get; set; }
        public IFormFile? Media { get; set; }

        public string? MediaUrl { get; set; }  // Harici URL seçeneği
    }
}
