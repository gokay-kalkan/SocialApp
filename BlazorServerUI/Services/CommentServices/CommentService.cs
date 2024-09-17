using Blazored.LocalStorage;
using BlazorServerUI.ApiResponseDto;
using BlazorServerUI.Data.CommentDtos;
using BlazorServerUI.StaticEndpoints;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace BlazorServerUI.Services.CommentServices
{
    public class CommentService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        private readonly ILocalStorageService _localStorage;

        public CommentService(HttpClient httpClient, IOptions<ApiSettings> apiSettings, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _localStorage = localStorage;
        }

        public async Task AddCommentAsync(CreateCommentDto createCommentDto)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            // Token'ı Authorization başlığına ekleyin
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.PostAsJsonAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.AddComment}", createCommentDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<GetCommentsForUserDto>> GetCommentsForFollowedUsersPostsAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            // Token'ı Authorization başlığına ekleyin
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // API çağrısı
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<GetCommentsForUserDto>>>(
                $"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.GetCommentsForFollowedUsersPosts}");

            // Eğer cevap null ise veya başarılı değilse bir hata fırlat
            if (response == null || !response.Success)
            {
                throw new Exception(response?.Message ?? "Yorumlar yüklenirken bir hata oluştu.");
            }

            return response.Data;
        }

        public async Task<List<GetCommentsForUserDto>> GetCommentsForUserPostsAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            // Token'ı Authorization başlığına ekleyin
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<GetCommentsForUserDto>>>(
                $"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.GetCommentsForUserPosts}");
            if (response != null && response.Success)
            {
                return response.Data;
            }
            return new List<GetCommentsForUserDto>();
        }

        public async Task<bool> DeleteCommentAsync(int commentId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.DeleteAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.DeleteComment}?commentId={commentId}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
                return result?.Success ?? false;
            }

            return false;
        }

        public async Task<ApiResponse<string>> UpdateCommentAsync(int commentId, string newContent)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var updateCommentDto = new UpdateCommentDto
            {
                Content = newContent
            };

            var response = await _httpClient.PutAsJsonAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.EditComment}?commentId={commentId}", updateCommentDto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
                return result ?? new ApiResponse<string>(true, "Yorum Güncellendi.", null);
            }

            return new ApiResponse<string>(false, "Yorum güncellenemedi.", null);
        }
    }
}
