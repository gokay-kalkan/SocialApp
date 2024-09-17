
using System.ComponentModel.DataAnnotations;

namespace DtoLayer.Dtos.UserDtos
{
    public enum Gender
    {
        Male = 1,
        Female = 2
    }
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public string ProfilePicture { get; set; }  

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }  // Doğum tarihi

        [Required]
        public Gender Gender { get; set; }
    }


}
