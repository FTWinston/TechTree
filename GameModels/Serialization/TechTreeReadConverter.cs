using ObjectiveStrategy.GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Serialization
{
    public class TechTreeReadConverter : JsonConverter<TechTree>
    {
        private class TechTreeDTO
        {
            [JsonConverter(typeof(UintDictionaryConverter<BuildingType>))]
            public Dictionary<uint, BuildingType>? Buildings { get; set; }
            
            [JsonConverter(typeof(UintDictionaryConverter<UnitType>))]
            public Dictionary<uint, UnitType>? Units { get; set; }

            [JsonConverter(typeof(UintDictionaryConverter<Research>))]
            public Dictionary<uint, Research>? Research { get; set; }
        }

        public override TechTree Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dto = JsonSerializer.Deserialize<TechTreeDTO>(ref reader, options);

            if (dto.Buildings == null || dto.Units == null || dto.Research == null)
                throw new JsonException();

            return new TechTree
            (
                dto.Buildings,
                dto.Units,
                dto.Research
            );
        }

        public override void Write(Utf8JsonWriter writer, TechTree value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
