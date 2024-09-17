using Blazored.LocalStorage;
using BlazorServerUI.ApiResponseDto;
using BlazorServerUI.Data.CommentDtos;
using BlazorServerUI.Data.PostDtos;
using BlazorServerUI.StaticEndpoints;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;

namespace BlazorServerUI.Services.PostServices
{
    public class PostService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        private readonly ILocalStorageService _localStorage;

        public PostService(HttpClient httpClient, IOptions<ApiSettings> apiSettings, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _localStorage = localStorage;
        }

        public async Task<List<FollowedUsersPostListDto>> GetFollowedUsersPostsAsync()
        {
            // LocalStorage'dan token'ı alın
            var token = await _localStorage.GetItemAsync<string>("authToken");

            // Token'ı Authorization başlığına ekleyin
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.GetFollowedUsersPosts}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Eğer 401 dönerse kullanıcıya bilgi verilebilir veya başka bir işlem yapılabilir
                throw new UnauthorizedAccessException("Unauthorized access, please check your token.");
            }

            response.EnsureSuccessStatusCode();

            // Yanıtı ApiResponse<List<FollowedUsersPostListDto>> olarak deserialize ediyoruz
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<FollowedUsersPostListDto>>>();

            // Data kısmını döndürüyoruz
            return apiResponse?.Data ?? new List<FollowedUsersPostListDto>();
        }

       
        public async Task<ApiResponse<string>> CreatePostAsync(MultipartFormDataContent content)
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                // API'ye gönderiyi POST isteği ile gönderiyoruz
                var response = await _httpClient.PostAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.CreatePost}", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
                    return result ?? new ApiResponse<string>(false, "Gönderi oluşturulurken beklenmeyen bir hata oluştu.", null);
                }

                return new ApiResponse<string>(false, "Gönderi oluşturulamadı.", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return new ApiResponse<string>(false, $"Gönderi oluşturulurken bir hata oluştu: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<string>> UpdatePostAsync(int postId, UpdatePostDto updatePostDto)
        {
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(updatePostDto.Content ?? string.Empty), "Content");

            if (updatePostDto.Media != null)
            {
                var streamContent = new StreamContent(updatePostDto.Media.OpenReadStream());
                formData.Add(streamContent, "Media", updatePostDto.Media.FileName);
            }

            var response = await _httpClient.PutAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.UpdatePost}?postId={postId}", formData);
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<string>(true, "Gönderi başarıyla güncellendi.", null);
            }
            return new ApiResponse<string>(false, "Güncelleme sırasında hata oluştu.", null);
        }


        public async Task<ApiResponse<List<PostListDto>>> GetUserPostsAsync()
        {
            try
            {
                // LocalStorage'dan token'ı alın
                var token = await _localStorage.GetItemAsync<string>("authToken");

                // Token'ı Authorization başlığına ekleyin
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                var response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.GetUserPosts}");
                if (response.IsSuccessStatusCode)
                {
                    var postList = await response.Content.ReadFromJsonAsync<ApiResponse<List<PostListDto>>>();
                    return postList ?? new ApiResponse<List<PostListDto>>(false, "Gönderiler bulunamadı.", null);
                }
                else
                {
                    return new ApiResponse<List<PostListDto>>(false, "Gönderiler alınırken hata oluştu.", null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return new ApiResponse<List<PostListDto>>(false, "Bir hata oluştu.", null);
            }
        }


        // PostService.cs
        public async Task<ApiResponse<List<FollowedUsersPostListDto>>> GetUserPostsIfFollowedAsync(string targetUserId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.GetUserPostsIfFollowed}?targetUserId={targetUserId}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<FollowedUsersPostListDto>>>();
                return result ?? new ApiResponse<List<FollowedUsersPostListDto>>(false, "Gönderiler yüklenemedi.", null);
            }

            return new ApiResponse<List<FollowedUsersPostListDto>>(false, "Gönderiler yüklenemedi.", null);
        }

        
        public async Task<ApiResponse<string>> DeletePostAsync(int postId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.DeleteAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.DeletePost}?postId={postId}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        }


    }
}


