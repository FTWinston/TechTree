using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Serialization
{
    public class ObjectiveCellsConverter : JsonConverter<Dictionary<int, int[]>?>
    {
        public override Dictionary<int, int[]>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new Dictionary<int, int[]>();
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<int, int[]>? value, JsonSerializerOptions options)
        {
            
        }
    }
}
