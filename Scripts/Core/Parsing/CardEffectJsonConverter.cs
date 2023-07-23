using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TFCardBattle.Core.Parsing
{
    public class CardEffectJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ICardEffect);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer
        )
        {
            // If the json value is an array, interpret it as a shorthand for
            // the "Multi" effect class.
            if (reader.TokenType == JsonToken.StartArray)
            {
                var array = JArray.Load(reader);
                var multi = new CardEffects.Multi();
                multi.Effects = CardEffectArrayJsonConverter.FromJArray(array);

                return multi;
            }

            var jobj = JObject.Load(reader);
            return FromJObject(jobj);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public static ICardEffect FromJObject(JObject obj)
        {
            dynamic dynamicEffect = obj;
            string className = dynamicEffect.Class ?? "Simple";

            var type = FindClass.InNamespace(
                "TFCardBattle.Core.CardEffects",
                className
            );

            if (type == null)
                throw new NotImplementedException($"No \"{className}\" card class found");

            return (ICardEffect)obj.ToObject(type);
        }
    }
}