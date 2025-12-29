using language_proficiency_blockchain.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace language_proficiency_blockchain.Tests;


[NotInParallel]
public class BlockchainTests: BaseIntegrationTest
{
    [Test]
    public async Task TestOne()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var blocksCount = dbContext.Blocks.Count();

        await Assert.That(blocksCount).IsEqualTo(1); 
    }
}