using JetBrains.Annotations;
using language_proficiency_blockchain.HashModels.Interfaces;

namespace language_proficiency_blockchain.requests;

[PublicAPI]
public record AddBlockRequest(
    BlockBase BlockBase
);

// [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
// [JsonDerivedType(typeof(HashableInstitutionV1), nameof(BlockTypeV1.Institution))]
// [JsonDerivedType(typeof(HashableTestV1), nameof(BlockTypeV1.Test))]
// [JsonDerivedType(typeof(HashableTestResultV1), nameof(BlockTypeV1.TestResult))]
// public abstract record AdditionalAddBlockData;
//
// public record AdditionalAddInstitutionData : AdditionalAddBlockData;