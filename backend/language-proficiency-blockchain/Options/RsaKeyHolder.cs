using System.Security.Cryptography;

namespace language_proficiency_blockchain.Options;

internal sealed class RsaKeyHolder
{
    public required RSA PrivateKey { get; init; } 
    public required RSA PublicKey { get; init; } 
}