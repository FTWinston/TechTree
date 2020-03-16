using ObjectiveStrategy.GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ObjectiveStrategy.GameModels.Serialization
{
    public class GameDefinitionReadConverter : PropertyDictionaryConverter<GameDefinition>
    {
        public override GameDefinition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            int? complexity = 0;
            int? seed = 0;
            int? turnLimit = 0;
            Battlefield? battlefield = null;
            TechTree? techTree = null;
            var objectives = new List<Objective>();

            ReadProperties(ref reader, new Dictionary<string, FieldReader>
            {
                {
                    nameof(GameDefinition.Seed), (ref Utf8JsonReader r) => {
                        if (r.TokenType != JsonTokenType.Number)
                            throw new JsonException();
                        seed = r.GetInt32();
                    }
                },
                {
                    nameof(GameDefinition.Complexity), (ref Utf8JsonReader r) =>
                    {
                        if (r.TokenType != JsonTokenType.Number)
                            throw new JsonException();
                        complexity = r.GetInt32();
                    }
                },
                {
                    nameof(GameDefinition.TurnLimit), (ref Utf8JsonReader r) =>
                    {
                        if (r.TokenType != JsonTokenType.Number)
                            throw new JsonException();
                        turnLimit = r.GetInt32();
                    }
                },
                {
                    nameof(GameDefinition.Battlefield), (ref Utf8JsonReader r) =>
                    {
                        if (r.TokenType != JsonTokenType.StartObject)
                            throw new JsonException();
                        battlefield = JsonSerializer.Deserialize<Battlefield>(ref r);
                    }
                },
                {
                    nameof(GameDefinition.TechTree), (ref Utf8JsonReader r) =>
                    {
                        if (r.TokenType != JsonTokenType.StartObject)
                            throw new JsonException();
                        techTree = JsonSerializer.Deserialize<TechTree>(ref r);
                    }
                },
                {
                    nameof(GameDefinition.Objectives), (ref Utf8JsonReader r) =>
                    {
                        if (r.TokenType != JsonTokenType.StartArray)
                            throw new JsonException();

                        r.Read();

                        while (r.TokenType != JsonTokenType.EndArray)
                            objectives.Add(JsonSerializer.Deserialize<Objective>(ref r));
                        
                        r.Read();
                    }
                },
            });

            if (!complexity.HasValue || !seed.HasValue || !turnLimit.HasValue || battlefield == null || techTree == null || objectives.Count == 0)
                throw new JsonException();

            return new GameDefinition
            (
                complexity: complexity.Value,
                seed: seed.Value,
                turnLimit: turnLimit.Value,
                techTree: techTree,
                battlefield: battlefield,
                objectives: objectives.ToArray()
            );
        }
    }
}
