using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObjectiveStrategy.GameModels.Serialization
{
    sealed class IntDictionaryConverter<TValue> : JsonConverter<Dictionary<int, TValue>?>
    {
        public override Dictionary<int, TValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringDictionary = JsonSerializer.Deserialize<Dictionary<string, TValue>?>(ref reader, options);

            if (stringDictionary == null)
                return null;

            var intDictionary = new Dictionary<int, TValue>();

            var enumerator = stringDictionary.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var element = enumerator.Current;
                intDictionary.Add(int.Parse(element.Key), element.Value);
            }

            return intDictionary;
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<int, TValue>? value, JsonSerializerOptions options)
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
