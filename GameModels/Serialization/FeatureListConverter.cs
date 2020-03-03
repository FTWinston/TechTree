using ObjectiveStrategy.GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Serialization
{
    public class FeatureListConverter : JsonConverter<List<Feature>>
    {
        public override List<Feature> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<FeatureDTO[]>(ref reader)
                .Select(feature => feature.ToFeature())
                .ToList();
        }

        public override void Write(Utf8JsonWriter writer, List<Feature> value, JsonSerializerOptions options)
        {
            var features = value
                .Select(feature => feature.ToDTO())
                .ToArray();

            // Don't pass in options when recursively calling Serialize.
            JsonSerializer.Serialize(writer, features);
        }
    }
}
