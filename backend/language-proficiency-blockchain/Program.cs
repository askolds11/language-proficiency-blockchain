using System.Text;
using System.Text.Json;
using language_proficiency_blockchain;
using language_proficiency_blockchain.Authorization;
using language_proficiency_blockchain.Database;
using language_proficiency_blockchain.Database.Models;
using language_proficiency_blockchain.Options;
using language_proficiency_blockchain.services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services
    .AddOptions<RsaOptions>()
    .Bind(builder.Configuration.GetSection(RsaOptions.Options))
    .ValidateDataAnnotations();

builder.Services
    .AddOptions<AuthenticationOptions>()
    .Bind(builder.Configuration.GetSection(AuthenticationOptions.Authentication))
    .ValidateDataAnnotations();

builder.Services.AddSingleton<IOptionsMonitor<RsaKeyHolder>, RsaKeyMonitor>();
builder.Services.AddSingleton<ICryptoService, CryptoService>();
builder.Services.AddScoped<BlockchainService>();
builder.Services.AddScoped<InternalService>();

builder.Services.AddHttpClient();
builder.Services.AddScoped<INodeHttpClient, NodeHttpClient>();
builder.Services.AddScoped<INodeRepository, NodeRepository>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var connectionStrings = builder.Configuration
    .GetSection(ConnectionStringsOptions.ConnectionStrings)
    .Get<ConnectionStringsOptions>();
if (connectionStrings == null)
{
    throw new Exception("ConnectionStrings not found");
}

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connectionStrings.AppDb));

// Add authentication services
builder.Services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();
builder.Services.AddScoped<IAuthenticationOptions>(sp =>
    new AuthenticationOptionsWrapper(sp.GetRequiredService<IOptions<AuthenticationOptions>>()));
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Configure JWT Authentication
var authOptions = builder.Configuration
    .GetSection(AuthenticationOptions.Authentication)
    .Get<AuthenticationOptions>();

if (authOptions == null)
{
    throw new Exception("Authentication options not found");
}

var key = Encoding.UTF8.GetBytes(authOptions.JwtSecret);
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = authOptions.JwtIssuer,
            ValidateAudience = true,
            ValidAudience = authOptions.JwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorizationPolicies();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = false;
    options.SerializerOptions.RespectNullableAnnotations = true;
    options.SerializerOptions.RespectRequiredConstructorParameters = true;
    options.SerializerOptions.PropertyNameCaseInsensitive = false;
});

var app = builder.Build();

// Apply database migrations at startup
//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    dbContext.Database.Migrate();
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Add CORS middleware before authentication
app.UseCors("AllowAll");

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("api").MapEndpoints();

app.Run();