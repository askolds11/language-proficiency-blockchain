using System.Data.Common;
using language_proficiency_blockchain.Database;
using language_proficiency_blockchain.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using Testcontainers.PostgreSql;
using TUnit.Core.Interfaces;

namespace language_proficiency_blockchain.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncInitializer, IAsyncDisposable
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:18-alpine")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithDatabase("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType ==
                                                                    typeof(DbContextOptions<AppDbContext>));

            if (dbContextDescriptor is not null)
            {
                services.Remove(dbContextDescriptor);
            }

            var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType ==
                                                                       typeof(DbConnection));
            if (dbConnectionDescriptor is not null)
            {
                services.Remove(dbConnectionDescriptor);
            }
            
            var rsaMonitorDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IOptionsMonitor<RsaKeyHolder>));
            if (rsaMonitorDescriptor is not null)
            {
                services.Remove(rsaMonitorDescriptor);
            }
            
            var rsaFixture = new RsaKeyFixture();
            services.AddSingleton(rsaFixture);
            services.AddSingleton<IOptionsMonitor<RsaKeyHolder>>(rsaFixture.RsaOptionsMonitor);

            services.AddSingleton<DbConnection>(_ =>
            {
                var connection = new NpgsqlConnection(_container.GetConnectionString());
                connection.Open();

                return connection;
            });

            services.AddDbContext<AppDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseNpgsql(connection);
            });
        });

        base.ConfigureWebHost(builder);
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    public new async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _container.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}