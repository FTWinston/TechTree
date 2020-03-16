using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Serialization
{
    public class TechTreeReadConverter : JsonConverter<TechTree>
    {
        public override TechTree Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var techTree = JsonSerializer.Deserialize<TechTree>(ref reader);

            return techTree;
        }

        public override void Write(Utf8JsonWriter writer, TechTree value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
