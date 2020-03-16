using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Serialization
{
    public abstract class PropertyDictionaryConverter<T> : JsonConverter<T>
    {
        public delegate void FieldReader(ref Utf8JsonReader reader);

        protected void ReadProperties(ref Utf8JsonReader reader, Dictionary<string, FieldReader> properties)
        {
            while (true)
            {
                reader.Read();

                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    reader.Read(); // end the current object
                    return;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException($"Expected a property name, but got {reader.TokenType}");

                var propertyName = reader.GetString();

                if (!properties.TryGetValue(propertyName, out FieldReader propertyReader))
                    throw new JsonException($"Unexpected property name: {propertyName}");

                propertyReader(ref reader);
            }
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
