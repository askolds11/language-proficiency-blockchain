using System.Text.Json;
using language_proficiency_blockchain;
using language_proficiency_blockchain.services;
using language_proficiency_blockchain.data;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.AspNetCore.Http.Json;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<ExampleService>();
builder.Services.AddSingleton<CryptoService>();
builder.Services.AddScoped<BlockchainService>();

// Persistence: PostgreSQL (connection string from configuration or environment)
var connString = builder.Configuration.GetConnectionString("AppDb")
                 ?? "Host=localhost;Port=5432;Database=language_proficiency_blockchain;Username=postgres;Password=postgres";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connString));

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = false;
    options.SerializerOptions.RespectNullableAnnotations = true;
    options.SerializerOptions.RespectRequiredConstructorParameters = true;
    options.SerializerOptions.PropertyNameCaseInsensitive = false;
});

var app = builder.Build();

// Ensure database exists and seed local node if empty
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();

    if (!await db.Nodes.AnyAsync())
    {
        var crypto = scope.ServiceProvider.GetRequiredService<CryptoService>();
        var localNode = new language_proficiency_blockchain.models.Node
        {
            PublicKeyPem = crypto.PublicKeyPem,
            Address = Environment.MachineName,
            IsApproved = true
        };
        db.Nodes.Add(localNode);
        await db.SaveChangesAsync();

        // Add a genesis block
        db.Blocks.Add(new language_proficiency_blockchain.models.Block
        {
            Index = 1,
            Type = language_proficiency_blockchain.models.BlockType.Genesis,
            RefId = Guid.Empty,
            DataHash = CryptoService.ComputeSha256Hex("genesis"),
            PrevHash = string.Empty,
            CreatedByNodeId = localNode.Id,
            SignatureBase64 = crypto.SignToBase64("genesis"),
            Timestamp = DateTime.UtcNow,
            Hash = CryptoService.ComputeSha256Hex("genesis")
        });
        await db.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.MapGroup("api").MapEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}