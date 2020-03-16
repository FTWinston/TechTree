using ObjectiveStrategy.GameModels.Definitions;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Serialization
{
    public class GameDefinitionReadConverter : JsonConverter<GameDefinition>
    {
        private class DTO
        {
            public int Complexity { get; set; }
            
            public int Seed { get; set; }
            
            public int TurnLimit { get; set; }

            public Battlefield? Battlefield { get; set; }

            public TechTree? TechTree { get; set; }

            public Objective[]? Objectives { get; set; }
        }

        public override GameDefinition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dto = JsonSerializer.Deserialize<DTO>(ref reader, options);

            if (dto.Battlefield == null || dto.TechTree == null || dto.Objectives == null || dto.Objectives.Length == 0)
                throw new JsonException();

            return new GameDefinition
            (
                complexity: dto.Complexity,
                seed: dto.Seed,
                turnLimit: dto.TurnLimit,
                techTree: dto.TechTree,
                battlefield: dto.Battlefield,
                objectives: dto.Objectives
            );
        }

        public override void Write(Utf8JsonWriter writer, GameDefinition value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
