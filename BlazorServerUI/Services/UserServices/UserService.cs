using Blazored.LocalStorage;
using BlazorServerUI.ApiResponseDto;
using BlazorServerUI.Data.UserDtos;
using BlazorServerUI.StaticEndpoints;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorServerUI.Services.UserServices
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        private readonly ILocalStorageService _localStorage;
        public UserService(HttpClient httpClient, IOptions<ApiSettings> apiSettings, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _localStorage = localStorage;
        }



        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            var url = $"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.Register}";

            var response = await _httpClient.PostAsJsonAsync(url, registerDto);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        public async Task<ApiResponse<List<UserProfileDto>>> GetOtherUsersAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            // Token'ı Authorization başlığına ekleyin
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<UserProfileDto>>>($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.GetOtherUsers}");
            return response ?? new ApiResponse<List<UserProfileDto>>(false, "Kullanıcılar alınamadı.", null);
        }


    }

}
