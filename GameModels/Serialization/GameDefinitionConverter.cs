using ObjectiveStrategy.GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Serialization
{
    public class GameDefinitionConverter : JsonConverter<GameDefinition>
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

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            string propertyName = reader.GetString();
            reader.Read();

            switch (propertyName)
            {
                case nameof(GameDefinition.Seed):
                    if (reader.TokenType != JsonTokenType.Number)
                        throw new JsonException();
                    seed = reader.GetInt32();
                    break;
                case nameof(GameDefinition.Complexity):
                    if (reader.TokenType != JsonTokenType.Number)
                        throw new JsonException();
                    complexity = reader.GetInt32();
                    break;
                case nameof(GameDefinition.TurnLimit):
                    if (reader.TokenType != JsonTokenType.Number)
                        throw new JsonException();
                    turnLimit = reader.GetInt32();
                    break;
                case nameof(GameDefinition.Battlefield):
                    if (reader.TokenType != JsonTokenType.StartObject)
                        throw new JsonException();
                    battlefield = JsonSerializer.Deserialize<Battlefield>(ref reader);
                    break;
                case nameof(GameDefinition.TechTree):
                    if (reader.TokenType != JsonTokenType.StartObject)
                        throw new JsonException();
                    techTree = JsonSerializer.Deserialize<TechTree>(ref reader);
                    break;
                case nameof(GameDefinition.Objectives):
                    if (reader.TokenType != JsonTokenType.StartArray)
                        throw new JsonException();

                    reader.Read();

                    while (reader.TokenType != JsonTokenType.EndArray)
                        objectives.Add(JsonSerializer.Deserialize<Objective>(ref reader));

                    break;
                default:
                    throw new JsonException();
            }

            reader.Read(); // end the current object

            if (!complexity.HasValue || !seed.HasValue || !turnLimit.HasValue || battlefield == null || techTree == null || objectives.Count == 0)
                throw new JsonException();

            return new GameDefinition(complexity.Value, seed.Value, turnLimit.Value, techTree, battlefield, objectives.ToArray());
        }

        public override void Write(Utf8JsonWriter writer, GameDefinition value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(GameDefinition.Seed), value.Seed);

            writer.WriteNumber(nameof(GameDefinition.Complexity), value.Complexity);

            writer.WriteNumber(nameof(GameDefinition.TurnLimit), value.TurnLimit);

            writer.WritePropertyName(nameof(value.TechTree));
            JsonSerializer.Serialize(writer, value.TechTree);

            writer.WritePropertyName(nameof(value.Battlefield));
            JsonSerializer.Serialize(writer, value.Battlefield);

            writer.WritePropertyName(nameof(value.Objectives));
            JsonSerializer.Serialize(writer, value.Objectives);
            
            writer.WriteEndObject();
        }
    }
}
