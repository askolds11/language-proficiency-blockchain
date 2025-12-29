using System.Text.Json;
using language_proficiency_blockchain.HashModels.Interfaces;
using language_proficiency_blockchain.services;

namespace language_proficiency_blockchain.HashModels.v1;

/// <summary>
/// Class for consistently hashing objects
/// </summary>
internal sealed class HasherV1: IHasher
{
    /// <summary>
    /// JsonSerializeOptions used for creating the JSON that is hashed
    /// </summary>
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        RespectNullableAnnotations = true,
        RespectRequiredConstructorParameters = true,
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Serializes an object to a text representation
    /// </summary>
    /// <param name="json">JSON to be deserialized</param>
    /// <typeparam name="T">Type of object</typeparam>
    /// <returns>Deserialized object</returns>
    public static T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions) ?? throw new ArgumentNullException(nameof(json), json);
    }
    
    /// <summary>
    /// Serializes an object to a text representation
    /// </summary>
    /// <param name="obj">Object to be serialized</param>
    /// <typeparam name="T">Type of object</typeparam>
    /// <returns>Serialized JSON</returns>
    public static string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, JsonSerializerOptions);
    }

    /// <inheritdoc/>
    public static byte[] CalculateHash<T>(T obj) where T : class, IHashable
    {
        var json = Serialize(obj);
        return CryptoService.ComputeSha256Hash(json);
    }
}