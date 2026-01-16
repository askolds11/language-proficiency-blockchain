using System.Collections.Concurrent;
using language_proficiency_blockchain.Database;
using language_proficiency_blockchain.Database.Models;
using language_proficiency_blockchain.HashModels.Interfaces;
using language_proficiency_blockchain.HashModels.v1;
using language_proficiency_blockchain.Options;
using language_proficiency_blockchain.responses.Blockchain;
using language_proficiency_blockchain.services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace language_proficiency_blockchain.Tests;

[NotInParallel]
internal class NodeRepositoryTests : BaseIntegrationTest
{
    [Test]
    public async Task CollectSignaturesAsync_ReturnsSignatures_WhenNodesRespond()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var mockHttpClient = Substitute.For<INodeHttpClient>();
        var signedHashBytes = new byte[] { 1, 2, 3, 4 };

        mockHttpClient.ProposeBlockAsync(
                Arg.Any<string>(),
                Arg.Any<BlockBase>(),
                Arg.Any<byte[]>(),
                Arg.Any<byte[]>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<ProposeBlockResponse?>(
                new ProposeBlockResponse(Convert.ToBase64String(signedHashBytes))));

        var mockCryptoService = Substitute.For<ICryptoService>();
        var nodeOptions = Microsoft.Extensions.Options.Options.Create(new NodeOptions { Address = "http://local:5000" });

        var repository = new NodeRepository(mockHttpClient, db, mockCryptoService, nodeOptions);

        // Create institutions with blocks
        var instId = Guid.NewGuid();
        var blockId = Guid.NewGuid();

        db.Blocks.Add(new BlockEntity
        {
            Id = blockId,
            Type = BlockType.Institution,
            PrevId = new Guid("019b6749-15c6-7d7d-b276-cea3b005f79f"),
            PrevHash = [],
            Hash = [1, 2, 3],
            InstitutionId = null,
            SignedHash = [],
            Timestamp = DateTimeOffset.UtcNow
        });

        db.Institutions.Add(new InstitutionEntity
        {
            Id = instId,
            BlockId = blockId,
            Address = "http://node1:5000",
            PublicKeyPem = []
        });

        await db.SaveChangesAsync();

        // Act
        var block = new HashableInstitutionV1([], Guid.NewGuid(), "Test");
        var result = await repository.CollectSignaturesAsync(
            block,
            [1, 2, 3],
            [4, 5, 6],
            "public-key-pem",
            threshold: 1,
            CancellationToken.None);

        // Assert
        await Assert.That(result.Count).IsEqualTo(1);
        await Assert.That(result.First().SignedHash).IsEquivalentTo(signedHashBytes);
    }

    [Test]
    public async Task CollectSignaturesAsync_CallsAllNodesInParallel()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var mockHttpClient = Substitute.For<INodeHttpClient>();
        var callAddresses = new ConcurrentBag<string>();

        mockHttpClient.ProposeBlockAsync(
                Arg.Any<string>(),
                Arg.Any<BlockBase>(),
                Arg.Any<byte[]>(),
                Arg.Any<byte[]>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                var address = ci.ArgAt<string>(0);
                callAddresses.Add(address);
                return Task.FromResult<ProposeBlockResponse?>(
                    new ProposeBlockResponse(Convert.ToBase64String([1, 2, 3])));
            });

        var mockCryptoService = Substitute.For<ICryptoService>();
        var nodeOptions = Microsoft.Extensions.Options.Options.Create(new NodeOptions { Address = "http://local:5000" });

        var repository = new NodeRepository(mockHttpClient, db, mockCryptoService, nodeOptions);

        // Create multiple institutions
        for (var i = 0; i < 3; i++)
        {
            var instId = Guid.NewGuid();
            var blockId = Guid.NewGuid();

            db.Blocks.Add(new BlockEntity
            {
                Id = blockId,
                Type = BlockType.Institution,
                PrevId = new Guid("019b6749-15c6-7d7d-b276-cea3b005f79f"),
                PrevHash = [],
                Hash = [1, 2, 3],
                InstitutionId = null,
                SignedHash = [],
                Timestamp = DateTimeOffset.UtcNow
            });

            db.Institutions.Add(new InstitutionEntity
            {
                Id = instId,
                BlockId = blockId,
                Address = $"http://node{i}:5000",
                PublicKeyPem = []
            });

            await db.SaveChangesAsync();
        }

        // Act
        var block = new HashableInstitutionV1([], Guid.NewGuid(), "Test");
        await repository.CollectSignaturesAsync(
            block,
            [1, 2, 3],
            [4, 5, 6],
            "public-key-pem",
            threshold: 3,
            CancellationToken.None);

        // Assert - All nodes should have been called
        await mockHttpClient.Received(3).ProposeBlockAsync(
            Arg.Any<string>(),
            Arg.Any<BlockBase>(),
            Arg.Any<byte[]>(),
            Arg.Any<byte[]>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
        await Assert.That(callAddresses.Count).IsEqualTo(3);
    }

    [Test]
    public async Task CollectSignaturesAsync_IgnoresFailedNodes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var mockHttpClient = Substitute.For<INodeHttpClient>();
        var callCount = 0;

        mockHttpClient.ProposeBlockAsync(
                Arg.Any<string>(),
                Arg.Any<BlockBase>(),
                Arg.Any<byte[]>(),
                Arg.Any<byte[]>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                Interlocked.Increment(ref callCount);
                var address = ci.ArgAt<string>(0);
                return address.Contains("node0")
                    ? Task.FromResult<ProposeBlockResponse?>(null)
                    : Task.FromResult<ProposeBlockResponse?>(
                        new ProposeBlockResponse(Convert.ToBase64String([1, 2, 3])));
            });

        var mockCryptoService = Substitute.For<ICryptoService>();
        var nodeOptions = Microsoft.Extensions.Options.Options.Create(new NodeOptions { Address = "http://local:5000" });

        var repository = new NodeRepository(mockHttpClient, db, mockCryptoService, nodeOptions);

        // Create two institutions
        for (var i = 0; i < 2; i++)
        {
            var instId = Guid.NewGuid();
            var blockId = Guid.NewGuid();

            db.Blocks.Add(new BlockEntity
            {
                Id = blockId,
                Type = BlockType.Institution,
                PrevId = new Guid("019b6749-15c6-7d7d-b276-cea3b005f79f"),
                PrevHash = [],
                Hash = [1, 2, 3],
                InstitutionId = null,
                SignedHash = [],
                Timestamp = DateTimeOffset.UtcNow
            });

            db.Institutions.Add(new InstitutionEntity
            {
                Id = instId,
                BlockId = blockId,
                Address = $"http://node{i}:5000",
                PublicKeyPem = []
            });
        }

        await db.SaveChangesAsync();

        // Act
        var block = new HashableInstitutionV1([], Guid.NewGuid(), "Test");
        var result = await repository.CollectSignaturesAsync(
            block,
            [1, 2, 3],
            [4, 5, 6],
            "public-key-pem",
            threshold: 1,
            CancellationToken.None);

        // Assert
        await Assert.That(result.Count).IsEqualTo(1);
        await Assert.That(callCount).IsEqualTo(2); // Both nodes were called
    }

    [Test]
    public async Task CollectSignaturesAsync_OnlyIncludesInstitutionsWithBlocks()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var mockHttpClient = Substitute.For<INodeHttpClient>();
        var calledAddresses = new ConcurrentBag<string>();

        mockHttpClient.ProposeBlockAsync(
                Arg.Any<string>(),
                Arg.Any<BlockBase>(),
                Arg.Any<byte[]>(),
                Arg.Any<byte[]>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                calledAddresses.Add(ci.ArgAt<string>(0));
                return Task.FromResult<ProposeBlockResponse?>(
                    new ProposeBlockResponse(Convert.ToBase64String([1, 2, 3])));
            });

        var mockCryptoService = Substitute.For<ICryptoService>();
        var nodeOptions = Microsoft.Extensions.Options.Options.Create(new NodeOptions { Address = "http://local:5000" });

        var repository = new NodeRepository(mockHttpClient, db, mockCryptoService, nodeOptions);

        // Create one institution with block, one without
        var blockId = Guid.NewGuid();

        db.Blocks.Add(new BlockEntity
        {
            Id = blockId,
            Type = BlockType.Institution,
            PrevId = new Guid("019b6749-15c6-7d7d-b276-cea3b005f79f"),
            PrevHash = [],
            Hash = [1, 2, 3],
            InstitutionId = null,
            SignedHash = [],
            Timestamp = DateTimeOffset.UtcNow
        });

        db.Institutions.Add(new InstitutionEntity
        {
            Id = Guid.NewGuid(),
            BlockId = blockId, // Has block
            Address = "http://node-with-block:5000",
            PublicKeyPem = []
        });

        db.Institutions.Add(new InstitutionEntity
        {
            Id = Guid.NewGuid(),
            BlockId = null, // No block
            Address = "http://node-without-block:5000",
            PublicKeyPem = []
        });

        await db.SaveChangesAsync();

        // Act
        var block = new HashableInstitutionV1([], Guid.NewGuid(), "Test");
        await repository.CollectSignaturesAsync(
            block,
            [1, 2, 3],
            [4, 5, 6],
            "public-key-pem",
            threshold: 1,
            CancellationToken.None);

        // Assert - Only the institution with a block should be called
        await mockHttpClient.Received(1).ProposeBlockAsync(
            Arg.Is<string>(a => a == "http://node-with-block:5000"),
            Arg.Any<BlockBase>(),
            Arg.Any<byte[]>(),
            Arg.Any<byte[]>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
        await Assert.That(calledAddresses.Count).IsEqualTo(1);
    }

    [Test]
    public async Task BroadcastFinalizedBlockAsync_CallsAllNodes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var mockHttpClient = Substitute.For<INodeHttpClient>();
        mockHttpClient.SendFinalizedBlockAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<object>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        var mockCryptoService = Substitute.For<ICryptoService>();
        var nodeOptions = Microsoft.Extensions.Options.Options.Create(new NodeOptions { Address = "http://local:5000" });

        var repository = new NodeRepository(mockHttpClient, db, mockCryptoService, nodeOptions);

        // Create institutions
        for (var i = 0; i < 3; i++)
        {
            var instId = Guid.NewGuid();
            var blockId = Guid.NewGuid();

            db.Blocks.Add(new BlockEntity
            {
                Id = blockId,
                Type = BlockType.Institution,
                PrevId = new Guid("019b6749-15c6-7d7d-b276-cea3b005f79f"),
                PrevHash = [],
                Hash = [1, 2, 3],
                InstitutionId = null,
                SignedHash = [],
                Timestamp = DateTimeOffset.UtcNow
            });

            db.Institutions.Add(new InstitutionEntity
            {
                Id = instId,
                BlockId = blockId,
                Address = $"http://node{i}:5000",
                PublicKeyPem = []
            });
        }

        await db.SaveChangesAsync();

        // Act
        await repository.BroadcastFinalizedBlockAsync(
            "blockchain/blocks/institution",
            new { TestData = "test" },
            CancellationToken.None);

        // Assert
        await mockHttpClient.Received(1).SendFinalizedBlockAsync(
            Arg.Is<string>(a => a == "http://node0:5000"),
            Arg.Any<string>(),
            Arg.Any<object>(),
            Arg.Any<CancellationToken>());
        await mockHttpClient.Received(1).SendFinalizedBlockAsync(
            Arg.Is<string>(a => a == "http://node1:5000"),
            Arg.Any<string>(),
            Arg.Any<object>(),
            Arg.Any<CancellationToken>());
        await mockHttpClient.Received(1).SendFinalizedBlockAsync(
            Arg.Is<string>(a => a == "http://node2:5000"),
            Arg.Any<string>(),
            Arg.Any<object>(),
            Arg.Any<CancellationToken>());
    }
}