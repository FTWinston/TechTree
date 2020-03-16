using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Serialization
{
    sealed class EnumDictionaryConverter<TKey, TValue> : JsonConverter<Dictionary<TKey, TValue>?>
        where TKey : struct, Enum
    {
        public override Dictionary<TKey, TValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringDictionary = JsonSerializer.Deserialize<Dictionary<string, TValue>?>(ref reader, options);

            if (stringDictionary == null)
                return null;

            var enumDictionary = new Dictionary<TKey, TValue>();

            var enumerator = stringDictionary.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var element = enumerator.Current;
                enumDictionary.Add(Enum.Parse<TKey>(element.Key), element.Value);
            }

            return enumDictionary;
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<TKey, TValue>? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            var stringDictionary = new Dictionary<string, TValue>(value.Count);
            foreach (var (k, v) in value)
                stringDictionary[k.ToString()] = v;

            JsonSerializer.Serialize(writer, stringDictionary, options);
        }
    }
}
