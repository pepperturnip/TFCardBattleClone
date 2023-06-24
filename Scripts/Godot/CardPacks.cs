using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TFCardBattle.Core;

using CardClasses = TFCardBattle.Core.CardClasses;

namespace TFCardBattle.Godot
{
    public static class CardPacks
    {
        private static readonly Dictionary<string, Type> _cardClassCache = new Dictionary<string, Type>();
        private static readonly Dictionary<string, Type> _consumableClassCache = new Dictionary<string, Type>();

        public static IEnumerable<ICard> Load(string name)
        {
            string filePath = $"res://CardPacks/{name}.json";
            string rawJson = FileAccess.GetFileAsString(filePath);

            return JArray.Parse(rawJson)
                .Cast<JObject>()
                .Select(FromJObject)
                .ToArray();
        }

        private static ICard FromJObject(JObject obj)
        {
            string className = obj.ContainsKey("Class")
                ? (string)obj["Class"]
                : "Simple";

            if (className == "Simple")
                return ParseSimple(obj);

            if (className == "DowngradeIfHeart")
                return ParseDowngradeIfHeart(obj);

            return ParseWithReflection(obj, className);
        }

        private static ICard ParseSimple(JObject obj)
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

            var card = (CardClasses.Simple)ParseWithReflection(obj, "Simple");
            card.Consumables = consumables;
            return card;
        }

        private static ICard ParseDowngradeIfHeart(JObject obj)
        {
            var header = obj.ToObject<CardHeader>();
            return new CardClasses.DowngradeIfHeart
            {
                Name = header.Name,
                TexturePath = header.TexturePath,
                PurchaseStats = header.PurchaseStats,

                StrongVersion = (CardClasses.Simple)ParseSimple((JObject)obj["StrongVersion"]),
                WeakVersion = (CardClasses.Simple)ParseSimple((JObject)obj["WeakVersion"]),
            };
        }

        private static ICard ParseWithReflection(JObject obj, string className)
        {
            var header = obj.ToObject<CardHeader>();

            var type = FindClass(
                className,
                "TFCardBattle.Core.CardClasses",
                _cardClassCache
            );

            if (type == null)
                throw new NotImplementedException($"No \"{className}\" card class found");

            ICard card = (ICard)obj.ToObject(type);
            card.Name = header.Name;
            card.TexturePath = header.TexturePath;
            card.PurchaseStats = header.PurchaseStats;

            return card;
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

        private class CardHeader
        {
            public string Name {get; set;}
            public string Image {get; set;}

            public int MinTF {get; set;}
            public int MaxTF {get; set;}

            public int BrainCost {get; set;}
            public int HeartCost {get; set;}
            public int SubCost {get; set;}
            public int OfferWeight {get; set;} = 1;

            public string TexturePath => $"res://Media/Cards/{Image}";

            public CardPurchaseStats PurchaseStats => new CardPurchaseStats
            {
                BrainCost = BrainCost,
                HeartCost = HeartCost,
                SubCost = SubCost,

                MinTF = MinTF,
                MaxTF = MaxTF,
                OfferWeight = OfferWeight
            };
        }
    }
}