using System.Security.Cryptography;
using System.Text;
using language_proficiency_blockchain.Options;
using Microsoft.Extensions.Options;

namespace language_proficiency_blockchain.services;

/// <summary>
/// Provides cryptographic utilities used by the application, including
/// hashing, signing with an RSA private key,
/// and signature verification using a provided public key in PEM format.
/// </summary>
internal sealed class CryptoService
{
    /// <summary>
    /// Private key
    /// </summary>
    private readonly IOptionsMonitor<RsaKeyHolder> _rsaOptions;
    
    private RSA Rsa => _rsaOptions.CurrentValue.PrivateKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="CryptoService"/> class.
    /// </summary>
    public CryptoService(IOptionsMonitor<RsaKeyHolder> rsaOptions)
    {
        _rsaOptions = rsaOptions;
    }

    /// <summary>
    /// Computes SHA-256 hash for the given input string.
    /// </summary>
    /// <param name="input">UTF-8 text to hash.</param>
    /// <returns>SHA-256 hash byte array.</returns>
    public static byte[] ComputeSha256Hash(string input)
    {
        return SHA256.HashData(Encoding.UTF8.GetBytes(input));
    }

    /// <summary>
    /// Signs the provided hash using the local RSA private key and returns the signed hash.
    /// </summary>
    /// <param name="hash">Hash to sign.</param>
    /// <returns>Hash signed with SHA-256.</returns>
    public byte[] SignHash(byte[] hash)
    {
        return Rsa.SignData(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    /// <summary>
    /// Verifies a signed hash for the given input data using a public key.
    /// </summary>
    /// <param name="data">Data to check</param>
    /// <param name="hash">Signed hash</param>
    /// <param name="publicKey">Public key</param>
    /// <returns><c>true</c> if signature is valid; otherwise <c>false</c>.</returns>
    public static bool VerifyHash(byte[] data, byte[] hash, byte[] publicKey)
    {
        using var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(publicKey, out _);
        return rsa.VerifyData(data, hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}
