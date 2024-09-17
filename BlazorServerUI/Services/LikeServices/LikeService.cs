using Blazored.LocalStorage;
using BlazorServerUI.ApiResponseDto;
using BlazorServerUI.Data.PostDtos;
using BlazorServerUI.StaticEndpoints;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;

namespace BlazorServerUI.Services.LikeServices
{
    // Services/LikeService.cs
    public class LikeService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly ApiSettings _apiSettings;

        public LikeService(HttpClient httpClient, IOptions<ApiSettings> apiSettings, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _localStorage = localStorage;
        }

        public async Task<ApiResponse<string>> LikePostAsync(int postId)
        {
            // Token'ı local storage'dan alın
            var token = await _localStorage.GetItemAsync<string>("authToken");

            // Authorization başlığı ekleyin
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PostAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.LikePost}?postId={postId}", null);

            // Hata durumlarını kontrol edin
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("Yetkisiz erişim, lütfen giriş yapın.");
            }

            // ApiResponse olarak yanıtı deserialize edin
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
            return apiResponse;
        }

        
        public async Task<ApiResponse<List<PostLikeDto>>> GetPostLikesAsync(int postId)
        {
            // Token'ı local storage'dan alın
            var token = await _localStorage.GetItemAsync<string>("authToken");

            // Authorization başlığı ekleyin
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<PostLikeDto>>>($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.GetPostLikes}?postId={postId}");
            return response;
        }

    }

}
