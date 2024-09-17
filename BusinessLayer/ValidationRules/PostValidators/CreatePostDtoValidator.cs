
using DtoLayer.Dtos.PostDtos;
using FluentValidation;

namespace BusinessLayer.ValidationRules.PostValidators
{
    public class CreatePostDtoValidator:AbstractValidator<CreatePostDto>
    {
        public CreatePostDtoValidator()
        {
            RuleFor(p=>p.Content).NotEmpty().WithMessage("İçerik Alanı Boş Geçilemez.");
        }
    }
}
