using Azure.Core;
using BusinessLayer.Abstract;
using BusinessLayer.Exceptions;
using BusinessLayer.ValidationRules.PostValidators;
using BusinessLayer.ValidationRules.RegisterValidators;
using DtoLayer.Dtos.ApiResponseDto;
using DtoLayer.Dtos.PostDtos;
using DtoLayer.Dtos.UserDtos;
using EntityLayer.Entities;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace SocialAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHostingEnvironment _env;
        private readonly FollowerService _followerService;

        public AccountController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IHostingEnvironment env, FollowerService followerService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
            _followerService = followerService;
        }

        [HttpGet("getUserProfile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            var user = await _userManager.FindByIdAsync(currentUserId);
            if (user == null)
            {
                return NotFound(new ApiResponse<string>(false, "Kullanıcı bulunamadı.", null));
            }

            var profileDto = new UserProfileDto
            {
                UserName = user.UserName,
                ProfilePicture = user.ProfilePicture,
                Name=user.Name,
                Surname=user.Surname,
                FollowersCount = _followerService.List(f => f.FollowingUserId == currentUserId).Count(),
                FollowingCount = _followerService.List(f => f.FollowerUserId == currentUserId).Count()
            };

            return Ok(new ApiResponse<UserProfileDto>(true, "Kullanıcı bilgileri başarıyla getirildi.", profileDto));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            

            var user = new User
            {
                
                UserName = model.Username,
                Name=model.Name,
                Surname=model.Surname,
                Email = model.Email,
                ProfilePicture = null,
                CreatedDate = DateTime.UtcNow,
                BirthDate = model.BirthDate?.Date,
                Gender= (EntityLayer.Entities.Gender)model.Gender,
            };

            IdentityRole role = new IdentityRole()
            {
                Name = "User"
            };

            await _roleManager.CreateAsync(role);

            var result = await _userManager.CreateAsync(user, model.Password);

            var resultt = await _userManager.AddToRoleAsync(user, role.Name);

            if (result.Succeeded || resultt.Succeeded)
            {
                return Ok(new ApiResponse<string>(true, "Kullanıcı Başarıyla Kayıt Oldu", user.Id));
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new ApiResponse<string>(false, $"Kullanıcı Kaydolurken Bir Hata oluştu: {errors}", null));
        }

        [HttpPut("uploadProfilePicture")]
        public async Task<IActionResult> UploadProfilePicture([FromForm] UploadProfilePictureDto uploadProfileDto)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            var user = await _userManager.FindByIdAsync(currentUserId);
            if (user == null)
            {
                return NotFound(new ApiResponse<string>(false, "Kullanıcı bulunamadı.", null));
            }

            if (uploadProfileDto.Image != null && uploadProfileDto.Image.Length > 0)
            {
                // Dosya ismini güvenli hale getiriyoruz
                var safeFileName = Path.GetFileNameWithoutExtension(uploadProfileDto.Image.FileName)
                    .Replace(" ", "-")
                    .Replace("ç", "c")
                    .Replace("ğ", "g")
                    .Replace("ı", "i")
                    .Replace("ö", "o")
                    .Replace("ş", "s")
                    .Replace("ü", "u")
                    .Replace("Ç", "C")
                    .Replace("Ğ", "G")
                    .Replace("İ", "I")
                    .Replace("Ö", "O")
                    .Replace("Ş", "S")
                    .Replace("Ü", "U")
                    + Path.GetExtension(uploadProfileDto.Image.FileName).ToLower();

                // Dosyayı 'uploads/posts' klasörüne kaydetme
                var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", "posts");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                var filePath = Path.Combine(uploadFolder, safeFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadProfileDto.Image.CopyToAsync(stream);
                }

                // Sunucunun doğru yolu döndürdüğünden emin olun
                var returnUrl = $"{Request.Scheme}://{Request.Host}/uploads/posts/{safeFileName}";
                user.ProfilePicture = returnUrl;

                await _userManager.UpdateAsync(user);

                return Ok(new ApiResponse<string>(true, "Profil resmi başarıyla yüklendi.", returnUrl));
            }

            return BadRequest(new ApiResponse<string>(false, "Dosya yüklenemedi.", null));
        }



        [HttpDelete("deleteProfilPicture")]
        public async Task<IActionResult> DeleteProfilePicture()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            var user = await _userManager.FindByIdAsync(currentUserId);
            if (user == null)
            {
                return NotFound(new ApiResponse<string>(false, "Kullanıcı bulunamadı.", null));
            }

            // Profil resmi var mı kontrol et
            if (!string.IsNullOrEmpty(user.ProfilePicture))
            {
                var path = Path.Combine(_env.WebRootPath, "uploads", user.ProfilePicture);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);  // Eski dosyayı sil
                }

                user.ProfilePicture = null; // Profil resmi alanını temizle
                await _userManager.UpdateAsync(user);
            }
            else
            {
                return BadRequest(new ApiResponse<string>(false, "Profil resmi bulunamadı.", null));
            }

            return Ok(new ApiResponse<string>(true, "Profil resmi başarıyla kaldırıldı.", null));
        }


        [HttpPut("updateProfilePicture")]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] UploadProfilePictureDto uploadProfileDto)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            var user = await _userManager.FindByIdAsync(currentUserId);
            if (user == null)
            {
                return NotFound(new ApiResponse<string>(false, "Kullanıcı bulunamadı.", null));
            }

            // Eski profil resmini silme işlemi (eğer bir resim varsa)
            if (!string.IsNullOrEmpty(user.ProfilePicture))
            {
                var oldPath = Path.Combine(_env.WebRootPath, "uploads", user.ProfilePicture);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            // Yeni dosya yükleme işlemi
            if (uploadProfileDto.Image != null && uploadProfileDto.Image.Length > 0)
            {
                var path = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path = Path.Combine(path, uploadProfileDto.Image.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await uploadProfileDto.Image.CopyToAsync(stream);
                }

                user.ProfilePicture = uploadProfileDto.Image.FileName; // Yeni profil resmini güncelleyin
                await _userManager.UpdateAsync(user);
            }
            else
            {
                return BadRequest(new ApiResponse<string>(false, "Geçerli bir dosya seçilmedi.", null));
            }

            return Ok(new ApiResponse<string>(true, "Profil resmi başarıyla güncellendi.", null));
        }


        [HttpGet("getFollowingUsers/{userId}")]
        public async Task<IActionResult> GetFollowingUsers(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new ApiResponse<string>(false, "Kullanıcı bulunamadı.", null));
            }

            var followingUsers = _followerService.List(f => f.FollowerUserId == userId)
                .Select(f => new UserProfileDto
                {
                    UserId = f.FollowingUserId,
                    UserName = f.FollowingUser.UserName,
                    ProfilePicture = f.FollowingUser.ProfilePicture
                }).ToList();

            return Ok(new ApiResponse<List<UserProfileDto>>(true, "Takip edilen kullanıcılar listelendi.", followingUsers));
        }

        [HttpGet("getFollowers/{userId}")]
        public async Task<IActionResult> GetFollowers(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new ApiResponse<string>(false, "Kullanıcı bulunamadı.", null));
            }

            var followers = _followerService.List(f => f.FollowingUserId == userId)
                .Select(f => new UserProfileDto
                {
                    UserId = f.FollowerUserId,
                    UserName = f.FollowerUser.UserName,
                    ProfilePicture = f.FollowerUser.ProfilePicture
                }).ToList();

            return Ok(new ApiResponse<List<UserProfileDto>>(true, "Takipçiler listelendi.", followers));
        }


        [HttpGet("getUserProfileById")]
        public async Task<IActionResult> GetUserProfileById(string userId)
        {
            // Kullanıcıyı ID'ye göre buluyoruz
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new ApiResponse<string>(false, "Kullanıcı bulunamadı.", null));
            }

            // Kullanıcının profil bilgilerini DTO'ya dönüştürüyoruz
            var userProfileDto = new UserProfileDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                ProfilePicture = user.ProfilePicture,
                FollowersCount = await _userManager.Users.CountAsync(u => u.Followers.Any(f => f.FollowingUserId == user.Id)),
                FollowingCount = await _userManager.Users.CountAsync(u => u.Following.Any(f => f.FollowerUserId == user.Id))
            };

            return Ok(new ApiResponse<UserProfileDto>(true, "Kullanıcı profili getirildi.", userProfileDto));
        }

        [HttpGet("getOtherUsers")]
        public async Task<IActionResult> GetOtherUsers()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            try
            {
                // Tüm kullanıcıları çekiyoruz
                var allUsers = await _userManager.Users
                    .Where(u => u.Id != currentUserId)
                    .AsNoTracking() // Performansı artırmak için tracking devre dışı bırakıldı
                    .ToListAsync();

                // Takip edilen kullanıcıların ID'lerini listeye ekliyoruz
                var followedUserIds = _followerService.List(f => f.FollowerUserId == currentUserId)
                    .Select(f => f.FollowingUserId)
                    .ToList();

                // Client-side'da filtrasyonu tamamlıyoruz
                var otherUsers = allUsers
                    .Where(u => !followedUserIds.Contains(u.Id))
                    .Select(user => new UserProfileDto
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        ProfilePicture = user.ProfilePicture
                    })
                    .ToList();

                return Ok(new ApiResponse<List<UserProfileDto>>(true, "Kullanıcılar başarıyla listelendi.", otherUsers));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return StatusCode(500, new ApiResponse<string>(false, $"Sunucu hatası: {ex.Message}", null));
            }
        }


        [HttpPut("updateUserName")]
        public async Task<IActionResult> UpdateUserName([FromBody] UpdateUserNameDto model)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "Kullanıcı giriş yapmamış.", null));
            }

            var user = await _userManager.FindByIdAsync(currentUserId);
            if (user == null)
            {
                return NotFound(new ApiResponse<string>(false, "Kullanıcı bulunamadı.", null));
            }

            user.Name = model.Name;
            user.Surname = model.Surname;
            user.UserName = model.UserName;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new ApiResponse<string>(true, "Ad ve soyad başarıyla güncellendi.", null));
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new ApiResponse<string>(false, $"Güncelleme sırasında hata oluştu: {errors}", null));
        }



    }
}
