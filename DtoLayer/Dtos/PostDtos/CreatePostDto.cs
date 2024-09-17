﻿

using Microsoft.AspNetCore.Http;

namespace DtoLayer.Dtos.PostDtos
{
    public class CreatePostDto
    {
        public string Content { get; set; }
        public IFormFile? Media { get; set; } // Dosya yükleme seçeneği
        public string? MediaUrl { get; set; }  // Harici URL seçeneği
    }

}