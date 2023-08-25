using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TFCardBattle.Core.Parsing
{
    public class LingeringEffectJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ILingeringEffect);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer
        )
        {
            var jobj = JObject.Load(reader);
            return FromJObject(jobj);
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer
        )
        {
            throw new NotImplementedException();
        }

        public static ILingeringEffect FromJObject(JObject obj)
        {
            dynamic dynamicEffect = obj;
            string className = dynamicEffect.Class;

            var type = FindClass.InNamespace(
                "TFCardBattle.Core.LingeringEffects",
                className
            );

            if (type == null)
                throw new NotImplementedException($"No \"{className}\" class found");

            return (ILingeringEffect)obj.ToObject(type);
        }
    }
}