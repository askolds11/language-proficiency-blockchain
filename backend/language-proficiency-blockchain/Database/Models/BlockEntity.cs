using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using language_proficiency_blockchain.HashModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace language_proficiency_blockchain.Database.Models;

/// <summary>
/// Represents a block in the blockchain containing immutable, signed data.
/// </summary>
internal class BlockEntity
{
    /// <summary>
    /// Unique identifier of the block.
    /// </summary>
    [Key]
    public required Guid Id { get; init; }

    /// <summary>
    /// The semantic type of the block (e.g., <see cref="BlockType.TestResult"/>).
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Column(TypeName = "varchar(50)")]
    public required BlockType Type { get; init; }

    /// <summary>
    /// Identifier of the previous block
    /// </summary>
    public required Guid PrevId { get; init; }

    /// <summary>
    /// Hash of the previous block (hex-encoded). Empty for the genesis block.
    /// </summary>
    public required byte[] PrevHash { get; init; }

    /// <summary>
    /// Hash of the current block (hex-encoded) calculated from the block contents.
    /// </summary>
    public required byte[] Hash { get; init; }

    /// <summary>
    /// Identifier of the institution that created and signed the block.
    /// </summary>
    public required Guid? InstitutionId { get; init; }

    /// <summary>
    /// Signed block hash signed by <see cref="InstitutionId"/>'s private key.
    /// </summary>
    public required byte[] SignedHash { get; init; }

    /// <summary>
    /// UTC timestamp when the block was created.
    /// </summary>
    public required DateTimeOffset Timestamp { get; init; }

    /// <summary>
    /// Navigation to the previous block entity.
    /// </summary>
    [ForeignKey(nameof(PrevId))]
    public BlockEntity PrevBlock { get; init; } = null!;

    /// <summary>
    /// Navigation to the related institution entity.
    /// </summary>
    [ForeignKey(nameof(InstitutionId))]
    public InstitutionEntity? Institution { get; init; }

    /// <summary>
    /// Navigation to the block's institution if relevant.
    /// </summary>
    [InverseProperty(nameof(InstitutionEntity.BlockId))]
    public InstitutionEntity? InstitutionData { get; } = null!;

    /// <summary>
    /// Navigation to the block's test if relevant.
    /// </summary>
    [InverseProperty(nameof(TestEntity.BlockId))]
    public TestEntity? TestData { get; } = null!;

    /// <summary>
    /// Navigation to the block's test result if relevant.
    /// </summary>
    [InverseProperty(nameof(TestResultEntity.BlockId))]
    public TestResultEntity? TestResultData { get; } = null!;
}

internal class BlockConfiguration : IEntityTypeConfiguration<BlockEntity>
{
    public void Configure(EntityTypeBuilder<BlockEntity> builder)
    {
        var firstId = Guid.Parse("019b6749-15c6-7d7d-b276-cea3b005f79f");

        var versionBlock = new VersionBlockBase([], 1);
        var versionBlockHash = Hasher.HashBlock(versionBlock);
        
        var block = new BlockEntity
        {
            Id = firstId,
            Type = BlockType.FirstBlock,
            PrevId = firstId,
            PrevHash = [],
            Hash = versionBlockHash,
            InstitutionId = null,
            SignedHash = [],
            Timestamp = DateTimeOffset.UnixEpoch,
        };
        
        builder.HasData(block);
    }
}