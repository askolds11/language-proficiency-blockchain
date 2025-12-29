namespace language_proficiency_blockchain.HashModels.Interfaces;

/// <summary>
/// Interface for a class that can hash objects
/// </summary>
internal interface IHasher
{
    /// <summary>
    /// Calculates the hash of an object
    /// </summary>
    /// <param name="obj">Object to hash</param>
    /// <typeparam name="T">Type of object</typeparam>
    /// <returns>Hash of object's JSON</returns>
    public static abstract byte[] CalculateHash<T>(T obj) where T : class, IHashable;
}