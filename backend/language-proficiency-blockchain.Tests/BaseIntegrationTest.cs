using System.Data.Common;
using language_proficiency_blockchain.Database;
using language_proficiency_blockchain.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Respawn.Graph;
using TUnit.Core.Interfaces;

namespace language_proficiency_blockchain.Tests;

public abstract class BaseIntegrationTest : IAsyncInitializer
{
    [ClassDataSource<TestWebApplicationFactory>(Shared = SharedType.PerClass)]
    public required TestWebApplicationFactory Factory { get; init; }

    private Respawner? _respawner;

    public async Task InitializeAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if ((await db.Database.GetPendingMigrationsAsync()).Any())
        {
            await db.Database.MigrateAsync();
        }

        var connection = scope.ServiceProvider.GetRequiredService<DbConnection>();
        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = [new Table("__EFMigrationsHistory")],
        });
    }

    [Before(Test)]
    public async Task ResetDatabaseAsync()
    {
        if (_respawner is null)
        {
            throw new InvalidOperationException("TestApplication is not initialized.");
        }

        using var scope = Factory.Services.CreateScope();
        var connection = scope.ServiceProvider.GetRequiredService<DbConnection>();
        await _respawner.ResetAsync(connection);
        
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (!db.Blocks.Any())
        {
            var block = new BlockEntity
            {
                Id = new Guid("019b6749-15c6-7d7d-b276-cea3b005f79f"),
                Hash =
                [
                    212, 66, 182, 77, 86, 235, 67, 91, 113, 74, 58, 132, 205, 131, 118, 95, 130, 154, 225, 220, 52, 170,
                    117, 29, 80, 22, 92, 198, 205, 247, 142, 2
                ],
                PrevId = new Guid("019b6749-15c6-7d7d-b276-cea3b005f79f"),
                InstitutionId = null,
                SignedHash = [],
                PrevHash = [],
                Timestamp = DateTimeOffset.UnixEpoch,
                Type = BlockType.FirstBlock
            };
            db.Add(block);
            await db.SaveChangesAsync();
        }
    }
}