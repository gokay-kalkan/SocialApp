using Blazored.LocalStorage;
using BlazorServerUI.ApiResponseDto;
using BlazorServerUI.Data.NotificationsDtos;
using BlazorServerUI.StaticEndpoints;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace BlazorServerUI.Services.NotificationServices
{
    public class FollowRequestNotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly ApiSettings _apiSettings;

        public FollowRequestNotificationService(HttpClient httpClient, ILocalStorageService localStorage, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _apiSettings = apiSettings.Value;
        }

        public async Task<ApiResponse<List<FollowRequestNotificationListDto>>> GetUserFollowRequestNotificationsAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.GetUserFollowRequestNotifications}");
            if (response.IsSuccessStatusCode)
            {
                var notifications = await response.Content.ReadFromJsonAsync<ApiResponse<List<FollowRequestNotificationListDto>>>();
                if (notifications != null && notifications.Data != null)
                {
                    // Status ve IsFollowedBack alanlarını buraya ekleyerek güncellemeyi sağlayın
                    foreach (var notification in notifications.Data)
                    {
                        // Örneğin: Eğer onaylanmışsa, status alanını "Approved" yapın
                        notification.Status = "Pending"; // İlk durum varsayılan olarak bekleme
                                                         // Duruma göre güncellemeleri buraya ekleyebilirsiniz.
                    }
                }
                return notifications;
            }

            return new ApiResponse<List<FollowRequestNotificationListDto>>(false, "Bildirimler yüklenirken hata oluştu.", null);
        }




    }

}
