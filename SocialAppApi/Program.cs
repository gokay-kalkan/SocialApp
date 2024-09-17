using DataAccessLayer.Database;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BusinessLayer.ServiceRegistirations;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BusinessLayer.Middlewares;
using FluentValidation.AspNetCore;
using BusinessLayer;
using FluentValidation;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
    

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(opt =>
{
    opt.SignIn.RequireConfirmedPhoneNumber = false;
    opt.SignIn.RequireConfirmedEmail = false;
    opt.SignIn.RequireConfirmedAccount = false;

    opt.Password.RequireDigit = false;
    opt.Password.RequiredLength = 8;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.User.AllowedUserNameCharacters = "abcçdefgðhýijklmnroöprsþtuüvyzABCÇDEFGÐHIÝJKLMNROÖPRSÞTUÜVYZ0123456789-._";
});

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = false,
        ValidateAudience = false

    };
});

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

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SocialApp", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Swaggerda Yetkilendirme",
        Type = SecuritySchemeType.ApiKey,
        Name = "Authorization",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    var key = new OpenApiSecurityScheme()
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        },
        In = ParameterLocation.Header
    };
    var requirement = new OpenApiSecurityRequirement
                    {
                             { key, new List<string>() }
                    };
    c.AddSecurityRequirement(requirement);
});

builder.Services.AddAuthorization();
builder.Services.AddDbContext<DataContext>(conf => conf.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")).UseLazyLoadingProxies());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegistrationServiceBusiness();

var app = builder.Build();



app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthorization();



app.MapControllers();

app.Run();
