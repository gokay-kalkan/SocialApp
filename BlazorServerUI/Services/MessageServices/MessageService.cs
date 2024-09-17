using Blazored.LocalStorage;
using BlazorServerUI.ApiResponseDto;
using BlazorServerUI.Data.MessageDtos;
using BlazorServerUI.StaticEndpoints;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace BlazorServerUI.Services.MessageServices
{
    public class MessageService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        private readonly ILocalStorageService _localStorage;
        public MessageService(HttpClient httpClient, IOptions<ApiSettings> apiSettings, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _localStorage = localStorage;
        }

        public async Task<ApiResponse<List<MessageThreadDto>>> GetMessageThreadsAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<MessageThreadDto>>>($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.GetUserMessageThreads}");
            return response;
        }

        public async Task<ApiResponse<List<MessageDto>>> GetMessagesAsync(string userId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<MessageDto>>>($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.GetUserMessages}?receiverId={userId}");
            return response;
        }

        public async Task<ApiResponse<string>> SendMessageAsync(MultipartFormDataContent content)
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.PostAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.SendMessage}", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
                    return result ?? new ApiResponse<string>(false, "Mesaj gönderilirken beklenmeyen bir hata oluştu.", null);
                }

                return new ApiResponse<string>(false, "Mesaj gönderilemedi.", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return new ApiResponse<string>(false, $"Mesaj gönderilirken bir hata oluştu: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<string>> UpdateMessageAsync(int messageId, UpdateMessageDto updateMessageDto)
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.PutAsJsonAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.UpdateMessage}?messageId={messageId}", updateMessageDto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
                    return result ?? new ApiResponse<string>(false, "Mesaj güncellenirken beklenmeyen bir hata oluştu.", null);
                }

                return new ApiResponse<string>(false, "Mesaj güncellenemedi.", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return new ApiResponse<string>(false, $"Mesaj güncellenirken bir hata oluştu: {ex.Message}", null);
            }
        }

        public async Task<int> GetUnreadMessageCountAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetFromJsonAsync<ApiResponse<int>>($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.GetUnreadMessageCount}");
            return response?.Data ?? 0;
        }


        public async Task<ApiResponse<string>> MarkAsReadAsync(int messageId)
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                // API'ye PUT isteği gönder
                var response = await _httpClient.PutAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.MarkAsRead}?messageId={messageId}", null);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
                    return result ?? new ApiResponse<string>(false, "Mesaj okunmuş olarak işaretlenirken beklenmeyen bir hata oluştu.", null);
                }

                return new ApiResponse<string>(false, "Mesaj okunmuş olarak işaretlenemedi.", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return new ApiResponse<string>(false, $"Mesaj okunmuş olarak işaretlenirken bir hata oluştu: {ex.Message}", null);
            }
        }


        public async Task<ApiResponse<string>> DeleteMessageAsync(int messageId)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.DeleteAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.DeleteMessage}?messageId={messageId}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        }


    }

}
