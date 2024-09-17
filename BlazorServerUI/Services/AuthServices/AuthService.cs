using Blazored.LocalStorage;
using BlazorServerUI.ApiResponseDto;
using BlazorServerUI.AuthenticationProvider;
using BlazorServerUI.Data.LoginResponseDtos;
using BlazorServerUI.Data.UserDtos;
using BlazorServerUI.StaticEndpoints;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

namespace BlazorServerUI.Services.AuthServices
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        private readonly ILocalStorageService _localStorage;
        private readonly CustomAuthStateProvider _customAuthStateProvider;

        public AuthService(HttpClient httpClient, IOptions<ApiSettings> apiSettings, ILocalStorageService localStorage, CustomAuthStateProvider customAuthStateProvider)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _localStorage = localStorage;
            _customAuthStateProvider = customAuthStateProvider;
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var url = $"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.Login}";
            var response = await _httpClient.PostAsJsonAsync(url, loginDto);
            response.EnsureSuccessStatusCode();

            // ApiResponse<LoginResponseDto> türünde deserialize edin
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDto>>();

            var loginResponse = apiResponse?.Data;
            var token = loginResponse?.Token;
            var userName = loginResponse?.UserName;

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userName))
            {
                await _localStorage.SetItemAsync("authToken", token);
                _customAuthStateProvider.MarkUserAsAuthenticated(userName);
            }

            return token;
        }




        public async Task LogoutAsync()
        {
            var url = $"{_apiSettings.BaseUrl}/{_apiSettings.Endpoints.Logout}";
            await _httpClient.PostAsync(url, null);

        }

    }
}
