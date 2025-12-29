using System.Security.Cryptography;
using language_proficiency_blockchain.services;
using TUnit.Assertions.Enums;

namespace language_proficiency_blockchain.Tests;

[ClassDataSource<RsaKeyFixture>(Shared = SharedType.PerClass)]
internal class CryptoServiceTests(RsaKeyFixture fixture)
{
    private const string RealData = "HASHME!@#$ĀČĒ";
    private const string RealHash = "8DF6308C18497EFDFC6CEC190773456B1D9D3C0E7025EFB805C326EA7E4A2B52";

    private const string RealSignedHash =
        "8169B8C66B34C9D5C664B56344B794596CAE72888591D1C67427AD164D0BD6531DA5DFA31BA3E6F6D4D6F65D9C45317B5D33B6214C0E443DA2EE59E73F2369A3";

    private static readonly byte[] RealHashBytes = Convert.FromHexString(RealHash);
    private static readonly byte[] RealSignedHashBytes = Convert.FromHexString(RealSignedHash);

    private const string FakeData = "FAKEĀ";
    private const string FakeHash = "FFE7C36EFB03778685C7B0E32777DCC77FCC6252BA63142CDF4B98E1131E9679";

    private const string FakeSignedHash =
        "6A1F158D818D316EF97FB265C1A8B6B2B8E0FBB7488F2D3FB5207141C30AFA08646EA18CD7422063EBFB5BA24789521E87F7F602E84C4C519A768D50BAAA4B30";

    private static readonly byte[] FakeHashBytes = Convert.FromHexString(FakeHash);
    private static readonly byte[] FakeSignedHashBytes = Convert.FromHexString(FakeSignedHash);

    [Test]
    public async Task TestHash()
    {
        var hash = CryptoService.ComputeSha256Hash(RealData);

        await Assert.That(hash).IsEquivalentTo(RealHashBytes, CollectionOrdering.Matching);
    }

    [Test]
    public async Task TestHashNotEquivalent()
    {
        var hash = CryptoService.ComputeSha256Hash(FakeData);

        await Assert.That(hash).IsNotEquivalentTo(RealHashBytes, CollectionOrdering.Matching);
    }

    [Test]
    public async Task TestSignHash()
    {
        var cryptoService = new CryptoService(fixture.RsaOptionsMonitor);

        var signedHash = cryptoService.SignHash(RealHashBytes);

        await Assert.That(signedHash).IsEquivalentTo(RealSignedHashBytes, CollectionOrdering.Matching);
    }

    [Test]
    public async Task TestSignHashNotEquivalent()
    {
        var cryptoService = new CryptoService(fixture.RsaOptionsMonitor);

        var signedHash = cryptoService.SignHash(FakeHashBytes);

        await Assert.That(signedHash).IsNotEquivalentTo(RealSignedHashBytes, CollectionOrdering.Matching);
    }

    [Test]
    public async Task TestVerifyHash()
    {
        var valid = CryptoService.VerifyHash(RealHashBytes, RealSignedHashBytes,
            fixture.PublicKey.ExportRSAPublicKey());

        await Assert.That(valid).IsTrue();
    }

    [Test]
    public async Task TestVerifyHashMismatch1()
    {
        var valid = CryptoService.VerifyHash(RealHashBytes, FakeSignedHashBytes,
            fixture.PublicKey.ExportRSAPublicKey());

        await Assert.That(valid).IsFalse();
    }

    [Test]
    public async Task TestVerifyHashMismatch2()
    {
        var valid = CryptoService.VerifyHash(FakeHashBytes, RealSignedHashBytes,
            fixture.PublicKey.ExportRSAPublicKey());

        await Assert.That(valid).IsFalse();
    }

    [Test]
    public async Task TestVerifyHashWrongKey()
    {
        var valid = CryptoService.VerifyHash(RealHashBytes, RealSignedHashBytes, RSA.Create(512).ExportRSAPublicKey());

        await Assert.That(valid).IsFalse();
    }
}