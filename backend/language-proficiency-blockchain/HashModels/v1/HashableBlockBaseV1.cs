using System.Text.Json.Serialization;
using language_proficiency_blockchain.HashModels.Interfaces;

namespace language_proficiency_blockchain.HashModels.v1;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(HashableInstitutionV1), nameof(BlockTypeV1.Institution))]
[JsonDerivedType(typeof(HashableTestV1), nameof(BlockTypeV1.Test))]
[JsonDerivedType(typeof(HashableTestResultV1), nameof(BlockTypeV1.TestResult))]
internal abstract record HashableBlockBaseV1(
    byte[] PrevHash
) : BlockBase(PrevHash, 1);