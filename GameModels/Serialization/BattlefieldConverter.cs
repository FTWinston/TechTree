using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Serialization
{
    public class BattlefieldConverter : JsonConverter<Battlefield>
    {
        public override Battlefield Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var battlefield = JsonSerializer.Deserialize<Battlefield>(ref reader);

            return battlefield;
        }

        public override void Write(Utf8JsonWriter writer, Battlefield value, JsonSerializerOptions options)
        {
            // Don't pass in options when recursively calling Serialize.
            // JsonSerializer.Serialize(writer, value);
            writer.WriteStartObject();
            writer.WriteEndObject();
        }
    }
}
