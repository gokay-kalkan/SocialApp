using Blazored.LocalStorage;
using BlazorServerUI.ApiResponseDto;
using BlazorServerUI.Data.UserDtos;
using BlazorServerUI.StaticEndpoints;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace BlazorServerUI.Services.UserProfileServices
{
    public class ProfileService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly ApiSettings _apiSettings;

        public ProfileService(HttpClient httpClient, ILocalStorageService localStorage, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _apiSettings = apiSettings.Value;
        }

        // Profil resmi yükleme metodu
        public async Task<ApiResponse<string>> UploadProfilePictureAsync(MultipartFormDataContent content)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PutAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.UploadProfilePicture}", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
            }

            return new ApiResponse<string>(false, "Profil resmi yüklenemedi.", null);
        }

        public async Task<ApiResponse<UserProfileDto>> GetUserProfileAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.GetUserProfile}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ApiResponse<UserProfileDto>>();
            }

            return new ApiResponse<UserProfileDto>(false, "Profil bilgileri getirilemedi.", null);
        }

        public async Task<ApiResponse<string>> DeleteProfilePictureAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.DeleteAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.DeleteProfilePicture}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
            }

            return new ApiResponse<string>(false, "Profil resmi silinemedi.", null);
        }

        // Takip edilen kullanıcıları alma metodu
        // Belirtilen kullanıcı ID'sine göre takip edilenleri getirme metodu
        public async Task<ApiResponse<List<UserProfileDto>>> GetFollowingUsersByUserIdAsync(string userId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.GetFollowingUsers}/{userId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ApiResponse<List<UserProfileDto>>>();
            }

            return new ApiResponse<List<UserProfileDto>>(false, "Takip edilen kullanıcılar getirilemedi.", null);
        }

        // Belirtilen kullanıcı ID'sine göre takipçileri getirme metodu
        public async Task<ApiResponse<List<UserProfileDto>>> GetFollowersByUserIdAsync(string userId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.GetFollowers}/{userId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ApiResponse<List<UserProfileDto>>>();
            }

            return new ApiResponse<List<UserProfileDto>>(false, "Takipçiler getirilemedi.", null);
        }


        // Belirli bir kullanıcının profilini almak için metot
        public async Task<ApiResponse<UserProfileDto>> GetUserProfileByIdAsync(string? userId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.GetUserProfileById}?userId={userId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ApiResponse<UserProfileDto>>();
            }

            return new ApiResponse<UserProfileDto>(false, "Kullanıcı profili getirilemedi.", null);
        }


        public async Task<ApiResponse<string>> UpdateUserNameAsync(UpdateUserNameDto updateUserNameDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.UpdateUserName}", updateUserNameDto);
            return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        }

    }
}
