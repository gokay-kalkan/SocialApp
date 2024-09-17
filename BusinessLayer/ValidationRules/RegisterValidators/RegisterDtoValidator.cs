

using DtoLayer.Dtos.UserDtos;
using FluentValidation;

namespace BusinessLayer.ValidationRules.RegisterValidators
{
    public class RegisterDtoValidator:AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
           RuleFor(x=>x.Email).NotEmpty().WithMessage("E Posta Alanı Boş Geçilemez");
           RuleFor(x=>x.Username).NotEmpty().WithMessage("E Posta Alanı Boş Geçilemez");
           RuleFor(x=>x.Name).NotEmpty().WithMessage("İsim Alanı Boş Geçilemez");
           RuleFor(x=>x.Surname).NotEmpty().WithMessage("Soyad Alanı Boş Geçilemez");
           RuleFor(x=>x.Password).NotEmpty().WithMessage("Şifre Alanı Boş Geçilemez");
        }
    }
}
