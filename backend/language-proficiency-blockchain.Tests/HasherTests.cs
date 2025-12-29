using language_proficiency_blockchain.HashModels;

namespace language_proficiency_blockchain.Tests;

public class HasherTests
{
    private static readonly VersionBlockBase VersionBlockBaseV1 =
        new(Convert.FromHexString("AA"), 1);

    private static readonly VersionBlockBase VersionBlockBaseV2 =
        new(Convert.FromHexString("AA"), 2);


    [Test]
    public async Task TestHashV1()
    {
        var hashFunc = () => Hasher.HashBlock(VersionBlockBaseV1);

        await Assert.That(hashFunc).ThrowsNothing();
    }

    [Test]
    public async Task TestHashV2Throws()
    {
        var hashFunc = () => Hasher.HashBlock(VersionBlockBaseV2);

        await Assert.That(hashFunc).Throws<ArgumentOutOfRangeException>();
    }
}