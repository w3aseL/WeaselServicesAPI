using DataAccessLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SpotifyAPILibrary;
using PortfolioLibrary;
using WeaselServicesAPI.Configuration;
using WeaselServicesAPI.Helpers;
using WeaselServicesAPI.Helpers.Interfaces;
using WeaselServicesAPI.Helpers.JWT;
using PortfolioLibrary.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// dbcontext
builder.Services.AddDbContext<ServicesAPIContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// email system
var emailConfig = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>();
builder.Services.AddSingleton<EmailSettings>(emailConfig);
builder.Services.AddScoped<IEmailSender, EmailSender>();

// jwt token generator
var jwtSettings = builder.Configuration.GetSection("JWTSettings").Get<JWTSettings>();
builder.Services.AddSingleton<JWTSettings>(jwtSettings);
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();

// spotify api
var spotifySettings = builder.Configuration.GetSection("SpotifySettings").Get<SpotifySettings>();
builder.Services.AddSingleton<SpotifySettings>(spotifySettings);
builder.Services.AddSingleton<SpotifySessionJobQueue>();
builder.Services.AddSingleton<SpotifyStateManager>();
builder.Services.AddSingleton<SpotifyClientFactory>(new SpotifyClientFactory(spotifySettings));
builder.Services.AddScoped<ISpotifyAPI, SpotifyAPILibrary.SpotifyAPI>();

// portfolio api
var s3Settings = builder.Configuration.GetSection("S3Settings").Get<S3Settings>();
builder.Services.AddSingleton<S3Settings>(s3Settings);
builder.Services.AddSingleton<S3Service>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
        opt => {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

// session tracking task
builder.Services.AddSingleton<SpotifyBackgroundTaskService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<SpotifyBackgroundTaskService>());

// session saving queue task
builder.Services.AddSingleton<SpotifySessionWriterTaskService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<SpotifySessionWriterTaskService>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == (int)System.Net.HttpStatusCode.Unauthorized)
    {
        await context.Response.WriteAsJsonAsync(new { Message = "Token Validation Has Failed. Request Access Denied" });
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
