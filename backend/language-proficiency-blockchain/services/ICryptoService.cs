namespace language_proficiency_blockchain.services;

internal interface ICryptoService
{
    /// <summary>
    /// Signs the provided hash using the local RSA private key and returns the signed hash.
    /// </summary>
    /// <param name="hash">Hash to sign.</param>
    /// <returns>Hash signed with SHA-256.</returns>
    byte[] SignHash(byte[] hash);
}