using ObjectiveStrategy.GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Serialization
{
    public class GameDefinitionConverter : JsonConverter<GameDefinition>
    {
        private static readonly JsonEncodedText Seed = JsonEncodedText.Encode(nameof(GameDefinition.Seed));
        private static readonly JsonEncodedText Complexity = JsonEncodedText.Encode(nameof(GameDefinition.Complexity));
        private static readonly JsonEncodedText TurnLimit = JsonEncodedText.Encode(nameof(GameDefinition.TurnLimit));
        private static readonly JsonEncodedText TechTree = JsonEncodedText.Encode(nameof(GameDefinition.TechTree));
        private static readonly JsonEncodedText Battlefield = JsonEncodedText.Encode(nameof(GameDefinition.Battlefield));
        private static readonly JsonEncodedText Objectives = JsonEncodedText.Encode(nameof(GameDefinition.Objectives));

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

            if (reader.ValueTextEquals(Seed.EncodedUtf8Bytes))
            {
                reader.Read();
                if (reader.TokenType != JsonTokenType.Number)
                    throw new JsonException();
                seed = reader.GetInt32();
            }
            else if (reader.ValueTextEquals(Complexity.EncodedUtf8Bytes))
            {
                reader.Read();
                if (reader.TokenType != JsonTokenType.Number)
                    throw new JsonException();
                complexity = reader.GetInt32();
            }
            else if (reader.ValueTextEquals(TurnLimit.EncodedUtf8Bytes))
            {
                reader.Read();
                if (reader.TokenType != JsonTokenType.Number)
                    throw new JsonException();
                turnLimit = reader.GetInt32();
            }
            else if (reader.ValueTextEquals(Battlefield.EncodedUtf8Bytes))
            {
                reader.Read();
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();
                battlefield = JsonSerializer.Deserialize<Battlefield>(ref reader);
            }
            else if (reader.ValueTextEquals(TechTree.EncodedUtf8Bytes))
            {
                reader.Read();
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();
                techTree = JsonSerializer.Deserialize<TechTree>(ref reader);
            }
            else if (reader.ValueTextEquals(Objectives.EncodedUtf8Bytes))
            {
                reader.Read();

                if (reader.TokenType != JsonTokenType.StartArray)
                    throw new JsonException();

                reader.Read();

                while (reader.TokenType != JsonTokenType.EndArray)
                    objectives.Add(JsonSerializer.Deserialize<Objective>(ref reader));
            }
            else
                throw new JsonException();

            reader.Read(); // end the current object

            if (!complexity.HasValue || !seed.HasValue || !turnLimit.HasValue || battlefield == null || techTree == null || objectives.Count == 0)
                throw new JsonException();

            return new GameDefinition(complexity.Value, seed.Value, turnLimit.Value, techTree, battlefield, objectives.ToArray());
        }

        public override void Write(Utf8JsonWriter writer, GameDefinition value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(Seed, value.Seed);

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
