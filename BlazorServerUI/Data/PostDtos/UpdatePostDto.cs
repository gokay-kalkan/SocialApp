namespace BlazorServerUI.Data.PostDtos
{
    public class UpdatePostDto
    {
        public string? Content { get; set; }
        public IFormFile? Media { get; set; }
        public string? ImageUrl { get; set; } // Eğer URL üzerinden güncellemek istenirse
    }
}
