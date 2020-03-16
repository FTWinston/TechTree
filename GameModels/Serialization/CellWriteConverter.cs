using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Serialization
{
    public class CellWriteConverter : JsonConverter<Cell>
    {
        public override Cell Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Cell value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(Cell.Type), (int)value.Type);

            if (value.Building != null)
            {
                writer.WritePropertyName(nameof(Cell.Building));
                JsonSerializer.Serialize(value.Building, options);
            }

            if (value.Units.Count > 0)
            {
                writer.WritePropertyName(nameof(Cell.Units));
                JsonSerializer.Serialize(value.Units, options);
            }

            writer.WriteEndObject();
        }
    }
}
