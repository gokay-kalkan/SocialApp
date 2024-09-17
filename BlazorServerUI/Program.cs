using Blazored.LocalStorage;
using BlazorServerUI.AuthenticationProvider;
using BlazorServerUI.Data;
using BlazorServerUI.Services.AuthServices;
using BlazorServerUI.Services.CommentServices;
using BlazorServerUI.Services.FollowerServices;
using BlazorServerUI.Services.LikeServices;
using BlazorServerUI.Services.MessageServices;
using BlazorServerUI.Services.NotificationServices;
using BlazorServerUI.Services.PostServices;
using BlazorServerUI.Services.RegisterServices;
using BlazorServerUI.Services.UserProfileServices;
using BlazorServerUI.Services.UserServices;
using BlazorServerUI.StaticEndpoints;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();


builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddScoped<ApiSettings>();
builder.Services.AddHttpClient<UserService>();
builder.Services.AddHttpClient<AuthService>();
builder.Services.AddHttpClient<PostService>();
builder.Services.AddHttpClient<CommentService>();
builder.Services.AddHttpClient<LikeService>();
builder.Services.AddHttpClient<ProfileService>();
builder.Services.AddHttpClient<FollowerService>();
builder.Services.AddHttpClient<FollowRequestNotificationService>();
builder.Services.AddHttpClient<NotificationService>();
builder.Services.AddHttpClient<MessageService>();
builder.Services.AddHttpClient<RegisterService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore(); // Bu, Blazor'da kimlik doðrulama sisteminin çalýþmasý için gerekli
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddBlazoredLocalStorage();

var app = builder.Build();
app.UseCors("AllowAllOrigins");
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseHttpsRedirection();



app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
