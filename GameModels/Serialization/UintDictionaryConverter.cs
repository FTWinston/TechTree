using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Serialization
{
    sealed class UintDictionaryConverter<TValue> : JsonConverter<Dictionary<uint, TValue>?>
    {
        public override Dictionary<uint, TValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringDictionary = JsonSerializer.Deserialize<Dictionary<string, TValue>?>(ref reader, options);

            if (stringDictionary == null)
                return null;

            var intDictionary = new Dictionary<uint, TValue>();

            var enumerator = stringDictionary.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var element = enumerator.Current;
                intDictionary.Add(uint.Parse(element.Key), element.Value);
            }

            return intDictionary;
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<uint, TValue>? value, JsonSerializerOptions options)
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
