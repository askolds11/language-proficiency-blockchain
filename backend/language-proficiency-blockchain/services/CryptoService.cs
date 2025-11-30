using System.Security.Cryptography;
using System.Text;

namespace language_proficiency_blockchain.services;

/// <summary>
/// Provides cryptographic utilities used by the application, including
/// hashing, signing with a locally generated ephemeral RSA key pair,
/// and signature verification using a provided public key in PEM format.
/// </summary>
public class CryptoService
{
    private readonly RSA _rsa;
    public string PublicKeyPem { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CryptoService"/> class and
    /// generates an ephemeral RSA key pair for the current process.
    /// </summary>
    public CryptoService()
    {
        // For demo/dev: generate ephemeral keypair on startup.
        _rsa = RSA.Create(2048);
        PublicKeyPem = ExportPublicKeyPem(_rsa);
    }

    /// <summary>
    /// Computes a lowercase hexadecimal SHA-256 hash for the given input string.
    /// </summary>
    /// <param name="input">UTF-8 text to hash.</param>
    /// <returns>Lowercase hex-encoded SHA-256 string.</returns>
    public static string ComputeSha256Hex(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    /// <summary>
    /// Signs the provided UTF-8 text using the local RSA private key and returns
    /// the signature as a Base64 string.
    /// </summary>
    /// <param name="input">Text to sign.</param>
    /// <returns>Base64-encoded PKCS#1 v1.5 signature over SHA-256.</returns>
    public string SignToBase64(string input)
    {
        var data = Encoding.UTF8.GetBytes(input);
        var sig = _rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return Convert.ToBase64String(sig);
    }

    /// <summary>
    /// Verifies a Base64 signature for the given input using a public key in PEM format.
    /// </summary>
    /// <param name="input">Original text that was signed.</param>
    /// <param name="signatureBase64">Base64-encoded signature to verify.</param>
    /// <param name="publicKeyPem">RSA public key in PEM format.</param>
    /// <returns><c>true</c> if signature is valid; otherwise <c>false</c>.</returns>
    public static bool VerifyWithPublicPem(string input, string signatureBase64, string publicKeyPem)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem);
        var data = Encoding.UTF8.GetBytes(input);
        var sig = Convert.FromBase64String(signatureBase64);
        return rsa.VerifyData(data, sig, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    /// <summary>
    /// Exports the provided RSA public key to PEM format.
    /// </summary>
    /// <param name="rsa">RSA instance containing the public key.</param>
    /// <returns>PEM-formatted public key string.</returns>
    private static string ExportPublicKeyPem(RSA rsa)
    {
        var sb = new StringBuilder();
        sb.AppendLine("-----BEGIN PUBLIC KEY-----");
        sb.AppendLine(Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo(), Base64FormattingOptions.InsertLineBreaks));
        sb.AppendLine("-----END PUBLIC KEY-----");
        return sb.ToString();
    }
}
