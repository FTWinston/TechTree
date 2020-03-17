using ObjectiveStrategy.GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Serialization
{
    public class BuildingTypeReadConverter : JsonConverter<BuildingType>
    {
        private class DTO : IBuildingType
        {
            public List<uint> Unlocks { get; set; } = new List<uint>();

            public List<uint> Builds { get; set; } = new List<uint>();

            public List<uint> Researches { get; set; } = new List<uint>();

            public int DisplayRow { get; set; }

            public int DisplayColumn { get; set; }

            public uint ID { get; set; }

            public int Health { get; set; }

            public int Armor { get; set; }

            public int Mana { get; set; }

            public int VisionRange { get; set; }

            public bool IsDetector { get; set; }

            public List<uint> UpgradesTo { get; set; } = new List<uint>();

            public uint? UpgradesFrom { get; set; }

            public List<Feature> Features { get; set; } = new List<Feature>();

            public List<Feature> LockedFeatures { get; set; } = new List<Feature>();

            public int BuildTime { get; set; }

            [JsonConverter(typeof(EnumDictionaryConverter<ResourceType, int>))]
            public Dictionary<ResourceType, int> Cost { get; set; } = new Dictionary<ResourceType, int>();

            public uint? Prerequisite { get; set; }

            public string Name { get; set; } = string.Empty;

            public string Symbol { get; set; } = string.Empty;
        }

        public override BuildingType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dto = JsonSerializer.Deserialize<DTO>(ref reader, options);

            if (string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Symbol))
                throw new JsonException();

            if (dto.Unlocks == null)
                dto.Unlocks = new List<uint>();

            if (dto.Builds == null)
                dto.Builds = new List<uint>();

            if (dto.Researches == null)
                dto.Researches = new List<uint>();

            if (dto.UpgradesTo == null)
                dto.UpgradesTo = new List<uint>();

            if (dto.Features == null)
                dto.Features = new List<Feature>();

            if (dto.LockedFeatures == null)
                dto.LockedFeatures = new List<Feature>();

            if (dto.Cost == null)
                dto.Cost = new Dictionary<ResourceType, int>();

            return new BuildingType(dto);
        }

        public override void Write(Utf8JsonWriter writer, BuildingType value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
