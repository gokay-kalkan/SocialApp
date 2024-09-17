using DtoLayer.Dtos.ApiResponseDto;
using DtoLayer.Dtos.LoginResponseDtos;
using DtoLayer.Dtos.UserDtos;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SocialAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(false, "Doğrulama Başarısız.", null));
            }

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı Bulunamadı.", null));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new ApiResponse<string>(false, "Login İşlemi Hatalı.", null));
            }

            var token = GenerateJwtToken(user);

            var roleName = _userManager.GetRolesAsync(user);

            var response = new LoginResponseDto
            {
                Token = token,
                UserName = user.UserName
            };

            return Ok(new ApiResponse<LoginResponseDto>(true, "Başarıyla Giriş Yaptınız.", response));
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var roles = _userManager.GetRolesAsync(user).Result;

            var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id),
    new Claim(ClaimTypes.Name, user.UserName)
};

            // Rolleri claim olarak ekliyoruz
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new ApiResponse<string>(true, "Başarıyla çıkış yapıldı.", null));
        }


    }
}
