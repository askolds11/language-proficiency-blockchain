using System.Text.Json;
using language_proficiency_blockchain;
using language_proficiency_blockchain.Database;
using language_proficiency_blockchain.Options;
using language_proficiency_blockchain.services;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services
    .AddOptions<RsaOptions>()
    .Bind(builder.Configuration.GetSection(RsaOptions.Options))
    .ValidateDataAnnotations();

builder.Services.AddSingleton<IOptionsMonitor<RsaKeyHolder>, RsaKeyMonitor>();
builder.Services.AddSingleton<ICryptoService, CryptoService>();
builder.Services.AddScoped<BlockchainService>();
builder.Services.AddScoped<InternalService>();

builder.Services.AddHttpClient();
builder.Services.AddScoped<INodeHttpClient, NodeHttpClient>();
builder.Services.AddScoped<INodeRepository, NodeRepository>();

var connectionStrings = builder.Configuration
    .GetSection(ConnectionStringsOptions.ConnectionStrings)
    .Get<ConnectionStringsOptions>();
if (connectionStrings == null)
{
    throw new Exception("ConnectionStrings not found");
}

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connectionStrings.AppDb));

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = false;
    options.SerializerOptions.RespectNullableAnnotations = true;
    options.SerializerOptions.RespectRequiredConstructorParameters = true;
    options.SerializerOptions.PropertyNameCaseInsensitive = false;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGroup("api").MapEndpoints();

app.Run();