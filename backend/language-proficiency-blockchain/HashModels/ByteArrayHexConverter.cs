using System.Text.Json;
using System.Text.Json.Serialization;

namespace language_proficiency_blockchain.HashModels;

internal sealed class ByteArrayHexConverter : JsonConverter<byte[]>
{
    public override byte[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var hex = reader.GetString();
        return hex is null ? null : Convert.FromHexString(hex);
    }

    public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
    {
        var hex = Convert.ToHexString(value);
        writer.WriteStringValue(hex);
    }
}