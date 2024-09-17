namespace BlazorServerUI.StaticEndpoints
{
    public static class ApiEndpoints
    {
        public const string BaseUrl = "https://localhost:7286";

        public static string Login => $"{BaseUrl}auth/login";
        // Diğer endpoint'leri de burada ekleyebilirsiniz
    }
}
