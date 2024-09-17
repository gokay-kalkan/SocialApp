using Blazored.LocalStorage;
using BlazorServerUI.ApiResponseDto;
using BlazorServerUI.Data.FollowersDtos;
using BlazorServerUI.Services.NotificationServices;
using BlazorServerUI.StaticEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace BlazorServerUI.Services.FollowerServices
{
    public class FollowerService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly ApiSettings _apiSettings;
        
        public FollowerService(HttpClient httpClient, ILocalStorageService localStorage, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _apiSettings = apiSettings.Value;
        }

        public async Task<ApiResponse<string>> UnfollowUserAsync(string userId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PostAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.UnfollowUser}?followingUserId={userId}", null);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
            }

            return new ApiResponse<string>(false, "Kullanıcı takipten çıkarılamadı.", null);
        }
        public async Task<ApiResponse<string>> FollowUserAsync(string targetUserId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            CreateFollowerDto followUserDto = new CreateFollowerDto { FollowingUserId = targetUserId };
            var response = await _httpClient.PostAsJsonAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.FollowUser}", followUserDto);

            return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        }

        public async Task<bool> IsFollowingAsync(string currentUserId, string targetUserId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/follower/checkfollowing?currentUserId={currentUserId}&targetUserId={targetUserId}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
                return result?.Data ?? false;
            }
            return false;
        }

        public async Task<ApiResponse<string>> ApproveFollowRequestAsync(int requestId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PostAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.ApproveFollowRequest}?requestId={requestId}", null);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
            }

            return new ApiResponse<string>(false, "Takip isteği onaylanamadı.", null);
        }


        // Takip isteğini reddetme metodu
        public async Task<ApiResponse<string>> RejectFollowRequestAsync(int requesterId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PostAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.RejectFollowRequest}?requestId={requesterId}", null);


            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
            }

            return new ApiResponse<string>(false, "Takip isteği reddedilemedi.", null);
        }
        public async Task<ApiResponse<string>> FollowBackAsync(string followerId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PostAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.FollowBack}?followerId={followerId}", null);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
            }

            return new ApiResponse<string>(false, "Geri takip etme isteği işlemi başarısız oldu.", null);
        }


        public async Task<ApiResponse<string>> ApproveFollowBackAsync(string followerId, int followRequestId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PostAsync($"{_apiSettings.BaseUrl}/follower/approveFollowBack?followerId={followerId}&followRequestId={followRequestId}", null);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
            }

            return new ApiResponse<string>(false, "Geri takip isteği onaylanamadı.", null);
        }

        public async Task<ApiResponse<string>> RejectFollowBackAsync(string followerId, int followRequestId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PostAsync($"{_apiSettings.BaseUrl}/follower/rejectFollowBack?followerId={followerId}&followRequestId={followRequestId}", null);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
            }

            return new ApiResponse<string>(false, "Geri takip isteği reddedilemedi.", null);
        }
        public async Task<bool> IsFollowRequestSentAsync(string currentUserId, string targetUserId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/follower/isFollowRequestSent?currentUserId={currentUserId}&targetUserId={targetUserId}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
                return result?.Data ?? false;
            }

            return false;
        }


    }
}
