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
        private static readonly Dictionary<string, Type> _effectClassCache = new Dictionary<string, Type>();

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

            var type = FindClass(
                className,
                "TFCardBattle.Core.CardEffects",
                _effectClassCache
            );

            if (type == null)
                throw new NotImplementedException($"No \"{className}\" card class found");

            return (ICardEffect)obj.ToObject(type);
        }

        private static Type FindClass(
            string className,
            string nameSpace,
            Dictionary<string, Type> cache
        )
        {
            if (cache.TryGetValue(className, out var result))
                return result;

            // Only search for classes in the the given namespace, for
            // security.  We don't want nefarious dudes instantiating any C#
            // class they want!
            var type = Assembly.GetExecutingAssembly()
                .DefinedTypes
                .Where(t => t.Namespace == nameSpace)
                .FirstOrDefault(t => t.Name == className);

            cache[className] = type;
            return type;
        }
    }
}