using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Serialization
{
    public class TechTreeConverter : JsonConverter<TechTree>
    {
        public override TechTree Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var techTree = JsonSerializer.Deserialize<TechTree>(ref reader);

            return techTree;
        }

        public override void Write(Utf8JsonWriter writer, TechTree value, JsonSerializerOptions options)
        {
            // Don't pass in options when recursively calling Serialize.
            // JsonSerializer.Serialize(writer, value);
            writer.WriteStartObject();
            writer.WriteEndObject();
        }
    }
}
