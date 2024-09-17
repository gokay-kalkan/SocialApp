using Blazored.LocalStorage;
using BlazorServerUI.ApiResponseDto;
using BlazorServerUI.Data.NotificationsDtos;
using BlazorServerUI.StaticEndpoints;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace BlazorServerUI.Services.NotificationServices
{
    public class NotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly ApiSettings _apiSettings;

        
        public NotificationService(HttpClient httpClient, ILocalStorageService localStorage, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _apiSettings = apiSettings.Value;
        }

        public async Task<int> GetNotificationCountAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.GetNotificationCount}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();
                return result?.Data ?? 0;
            }

            return 0;
        }

        public async Task<ApiResponse<List<NotificationListDto>>> GetUserAllNotificationsAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.GetUserAllNotifications}");
            if (response.IsSuccessStatusCode)
            {
                var notifications = await response.Content.ReadFromJsonAsync<ApiResponse<List<NotificationListDto>>>();
                return notifications;
            }
            return new ApiResponse<List<NotificationListDto>>(false, "Bildirimler yüklenirken hata oluştu.", null);
        }

      

    }
}
