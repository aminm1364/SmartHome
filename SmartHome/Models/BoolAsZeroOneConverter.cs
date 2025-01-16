using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartHome.Models
{
    public class BoolAsZeroOneConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32() == 1;
            }
            throw new JsonException("Invalid value for boolean property.");
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value ? 1 : 0);
        }
    }
}
