using BlazorServerUI.ApiResponseDto;
using BlazorServerUI.Data.UserDtos;
using BlazorServerUI.StaticEndpoints;
using Microsoft.Extensions.Options;

namespace BlazorServerUI.Services.RegisterServices
{
    public class RegisterService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        public RegisterService(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
        }

        public async Task<ApiResponse<string>> RegisterUserAsync(RegisterDto registerDto)
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;

            // API'ye POST isteği atıyoruz
            var response = await _httpClient.PostAsJsonAsync($"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.Register}", registerDto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
                return result;
            }
            else
            {
                var errorResponse = new ApiResponse<string>(
             success: false,
             message: "Kayıt işlemi başarısız oldu.",
             data: null
         );
                return errorResponse;
            }
        }
    }
}
