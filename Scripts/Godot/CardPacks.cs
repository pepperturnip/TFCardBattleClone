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
        public static IEnumerable<ICard> Load(string name)
        {
            string filePath = $"res://CardPacks/{name}.json";
            string rawJson = FileAccess.GetFileAsString(filePath);

            return JArray.Parse(rawJson)
                .Cast<JObject>()
                .Select(FromJObject);
        }

        private static ICard FromJObject(JObject obj)
        {
            string className = obj.ContainsKey("Class")
                ? (string)obj["Class"]
                : "Simple";

            if (className == "Simple")
                return ParseSimple(obj);

            return ParseWithReflection(obj);
        }

        private static ICard ParseSimple(JObject obj)
        {
            var c = obj.ToObject<SimpleJson>();
            var header = obj.ToObject<CardHeader>();

            return new CardClasses.Simple
            {
                Name = header.Name,
                TexturePath = header.TexturePath,
                PurchaseStats = header.PurchaseStats,

                Brain = c.Brain ?? 0,
                Heart = c.Heart ?? 0,
                Sub = c.Sub ?? 0,
                Shield = c.Shield ?? 0,
                Damage = c.Damage ?? 0,
                CardDraw = c.Draw ?? 0,
                SelfHeal = c.SelfHeal ?? 0,

                Consumables = c.Consumables == null
                    ? Array.Empty<IConsumable>()
                    : c.Consumables.Select(FromConsumableClass).ToArray()
            };
        }

        private static ICard ParseWithReflection(JObject obj)
        {
            var className = (string)obj["Class"];
            var header = obj.ToObject<CardHeader>();

            Type cardClassType = FindCardClass(className);
            if (cardClassType == null)
                throw new NotImplementedException($"No \"{className}\" card class found");

            ICard card = (ICard)obj.ToObject(cardClassType);
            card.Name = header.Name;
            card.TexturePath = header.TexturePath;
            card.PurchaseStats = header.PurchaseStats;

            return card;
        }

        private static Type FindCardClass(string cardClass)
        {
            // Only search for classes in the CardClasses namespace, for
            // security.  We don't want nefarious dudes instantiating any C#
            // class they want!
            return Assembly.GetExecutingAssembly()
                .DefinedTypes
                .Where(t => t.Namespace == "TFCardBattle.Core.CardClasses")
                .FirstOrDefault(t => t.Name == cardClass);
        }

        private static IConsumable FromConsumableClass(string consumableClass)
        {
            // Only search for classes in the ConsumableClasses namespace, for
            // security.  We don't want nefarious dudes instantiating any C#
            // class they want!
            var type = Assembly.GetExecutingAssembly()
                .DefinedTypes
                .Where(t => t.Namespace == "TFCardBattle.Core.ConsumableClasses")
                .FirstOrDefault(t => t.Name == consumableClass);

            if (type == null)
                throw new NotImplementedException($"No \"{consumableClass}\" consumable class found");

            return (IConsumable)Activator.CreateInstance(type);
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

            public string TexturePath => $"res://ApolloSevenImages/cardgame/cards/{Image}";

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

        private class SimpleJson
        {
            public int? Brain {get; set;}
            public int? Heart {get; set;}
            public int? Sub {get; set;}
            public int? Damage {get; set;}
            public int? Shield {get; set;}
            public int? Draw {get; set;}
            public int? SelfHeal {get; set;}

            public string[] Consumables {get; set;}
        }
    }
}