using System.Text.Json;
using language_proficiency_blockchain.HashModels.v1;
using TUnit.Assertions.Enums;

namespace language_proficiency_blockchain.Tests;

public class HasherV1Tests
{
    private static readonly Guid DummyGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly HashableInstitutionV1 HashableInstitutionV1 =
        new(Convert.FromHexString("AA"), DummyGuid, "Institution");
    private static readonly HashableTestV1 HashableTestV1 =
        new(Convert.FromHexString("AA"), DummyGuid, DummyGuid, "MaxScore");
    private static readonly HashableTestResultV1 HashableTestResultV1 =
        new(Convert.FromHexString("AA"), DummyGuid, DummyGuid, JsonSerializer.SerializeToDocument(new {test = 1}));

    private static readonly byte[] HashableInstitutionV1Hash =
        Convert.FromHexString("5BFD2986CB0844A321100EF42C836B4CF233885299F1E6672EB68DD7B54025CD");
    private static readonly byte[] HashableTestV1Hash =
        Convert.FromHexString("9B0AE8173D8501AACE20BD74093E56C79675731B781E4E73122049B0D204CC5C");
    private static readonly byte[] HashableTestResultV1Hash =
        Convert.FromHexString("4B5175BE7BD48669E2E2B00F574285D39E0EBB97ABE42F8DF36437750BF10CCD");

    [Test]
    public async Task TestHashInstitution()
    {
        var hash = HasherV1.CalculateHash(HashableInstitutionV1);

        PrintHash(hash);

        await Assert.That(hash).IsEquivalentTo(HashableInstitutionV1Hash, CollectionOrdering.Matching);
    }
    
    [Test]
    public async Task TestHashTest()
    {
        var hash = HasherV1.CalculateHash(HashableTestV1);

        PrintHash(hash);

        await Assert.That(hash).IsEquivalentTo(HashableTestV1Hash, CollectionOrdering.Matching);
    }
    
    [Test]
    public async Task TestHashTestResult()
    {
        var hash = HasherV1.CalculateHash(HashableTestResultV1);

        PrintHash(hash);

        await Assert.That(hash).IsEquivalentTo(HashableTestResultV1Hash, CollectionOrdering.Matching);
    }

    [Test]
    public async Task TestJsonTypeInstitution()
    {
        HashableBlockBaseV1 block = HashableInstitutionV1;

        var json = HasherV1.Serialize(block);

        Console.WriteLine(json);

        var deserialized = HasherV1.Deserialize<HashableBlockBaseV1>(json);

        await Assert.That(deserialized).IsTypeOf<HashableInstitutionV1>();
    }
    
    [Test]
    public async Task TestJsonTypeTest()
    {
        HashableBlockBaseV1 block = HashableTestV1;

        var json = HasherV1.Serialize(block);

        Console.WriteLine(json);

        var deserialized = HasherV1.Deserialize<HashableBlockBaseV1>(json);

        await Assert.That(deserialized).IsTypeOf<HashableTestV1>();
    }
    
    [Test]
    public async Task TestJsonTypeTestResult()
    {
        HashableBlockBaseV1 block = HashableTestResultV1;

        var json = HasherV1.Serialize(block);

        Console.WriteLine(json);

        var deserialized = HasherV1.Deserialize<HashableBlockBaseV1>(json);

        await Assert.That(deserialized).IsTypeOf<HashableTestResultV1>();
    }

    private static void PrintHash(byte[] hash)
    {
        Console.WriteLine(Convert.ToHexString(hash));
    }
}