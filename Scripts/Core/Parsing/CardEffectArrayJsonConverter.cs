using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TFCardBattle.Core.Parsing
{
    public class CardEffectArrayJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ICardEffect[]);
        }
        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer
        )
        {
            var array = JArray.Load(reader);
            return FromJArray(array);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public static ICardEffect[] FromJArray(JArray array)
        {
            return array
                .ToObject<JObject[]>()
                .Select(CardEffectJsonConverter.FromJObject)
                .ToArray();
        }
    }
}