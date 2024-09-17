

using Microsoft.AspNetCore.Http;

namespace DtoLayer.Dtos.UserDtos
{
    public class UploadProfilePictureDto
    {
        public IFormFile? Image { get; set; }
    }
}
