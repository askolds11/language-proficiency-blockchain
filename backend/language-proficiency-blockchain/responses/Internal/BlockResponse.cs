using JetBrains.Annotations;
using language_proficiency_blockchain.Database.Models;

namespace language_proficiency_blockchain.responses.Internal;

/// <summary>
/// Response DTO for a blockchain block.
/// </summary>
/// <param name="Id">Unique identifier of the block</param>
/// <param name="Type">The semantic type of the block</param>
/// <param name="PrevId">Identifier of the previous block</param>
/// <param name="PrevHashHex">Hash of the previous block (hex-encoded)</param>
/// <param name="HashHex">Hash of the current block (hex-encoded)</param>
/// <param name="InstitutionId">Identifier of the institution that created and signed the block</param>
/// <param name="SignedHashHex">Signed block hash (hex-encoded)</param>
/// <param name="Timestamp">UTC timestamp when the block was created</param>
[PublicAPI]
public sealed record BlockResponse(
    Guid Id,
    string Type,
    Guid PrevId,
    string PrevHashHex,
    string HashHex,
    Guid? InstitutionId,
    string SignedHashHex,
    DateTimeOffset Timestamp
)
{
    /// <summary>
    /// Creates a BlockResponse from a BlockEntity.
    /// </summary>
    internal static BlockResponse FromEntity(BlockEntity entity) => new(
        Id: entity.Id,
        Type: entity.Type.ToString(),
        PrevId: entity.PrevId,
        PrevHashHex: Convert.ToHexString(entity.PrevHash).ToLowerInvariant(),
        HashHex: Convert.ToHexString(entity.Hash).ToLowerInvariant(),
        InstitutionId: entity.InstitutionId,
        SignedHashHex: Convert.ToHexString(entity.SignedHash).ToLowerInvariant(),
        Timestamp: entity.Timestamp
    );
}

