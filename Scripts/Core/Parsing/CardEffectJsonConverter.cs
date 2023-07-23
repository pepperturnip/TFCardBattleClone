using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TFCardBattle.Core.Parsing
{
    public class CardEffectJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return
                objectType == typeof(ICardEffect) ||
                objectType == typeof(ICardEffect[]);
        }
        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer
        )
        {
            if (objectType == typeof(ICardEffect[]))
            {
                return JArray.Load(reader)
                    .ToObject<JObject[]>()
                    .Select(ParseWithReflection)
                    .ToArray();
            }

            var jobj = JObject.Load(reader);
            return ParseWithReflection(jobj);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private static ICardEffect ParseWithReflection(JObject obj)
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