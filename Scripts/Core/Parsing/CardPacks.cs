using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TFCardBattle.Core.Parsing
{
    public static class CardPacks
    {
        private static readonly Dictionary<string, Type> _effectClassCache = new Dictionary<string, Type>();
        private static readonly Dictionary<string, Type> _consumableClassCache = new Dictionary<string, Type>();

        public static IReadOnlyDictionary<string, Card> Parse(string rawJson)
        {
            var jObjs = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(rawJson);
            return jObjs.ToDictionary(kvp => kvp.Key, kvp => FromJObject(kvp.Value));
        }

        private static Card FromJObject(JObject obj)
        {
            var card = obj.ToObject<Card>();

            dynamic dynamicCard = obj;
            string effectClass = dynamicCard.Effect.Class ?? "Simple";

            if (effectClass == "Simple")
                card.Effect = ParseSimple((JObject)obj["Effect"]);
            else
                card.Effect = ParseWithReflection((JObject)obj["Effect"], effectClass);

            return card;
        }

        private static ICardEffect ParseSimple(JObject obj)
        {
            // HACK: Parse the consumables separately from the rest of the
            // card, since they're strings and we need IConsumables
            var consumableIds = obj.ContainsKey("Consumables")
                ? obj["Consumables"].ToObject<string[]>()
                : Array.Empty<string>();

            IConsumable[] consumables = consumableIds
                .Select(ParseConsumable)
                .ToArray();

            obj.Remove("Consumables");

            var card = (CardEffects.Simple)ParseWithReflection(obj, "Simple");
            card.Consumables = consumables;
            return card;
        }

        private static ICardEffect ParseWithReflection(JObject obj, string className)
        {
            var type = FindClass(
                className,
                "TFCardBattle.Core.CardEffects",
                _effectClassCache
            );

            if (type == null)
                throw new NotImplementedException($"No \"{className}\" card class found");

            return (ICardEffect)obj.ToObject(type);
        }

        private static IConsumable ParseConsumable(string className)
        {
            var type = FindClass(
                className,
                "TFCardBattle.Core.ConsumableClasses",
                _consumableClassCache
            );

            if (type == null)
                throw new NotImplementedException($"No \"{className}\" consumable class found");

            return (IConsumable)Activator.CreateInstance(type);
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