using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using Newtonsoft.Json;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public static class CardPacks
    {
        public static IEnumerable<ICard> Load(string name)
        {
            string filePath = $"res://CardPacks/{name}.json";
            string rawJson = FileAccess.GetFileAsString(filePath);
            var parsedJson = JsonConvert.DeserializeObject<SimpleCardJson[]>(rawJson);
            return parsedJson.Select(FromCardClass);
        }

        private static ICard FromCardClass(SimpleCardJson c)
        {
            string texturePath = $"res://ApolloSevenImages/cardgame/cards/{c.Image}";
            var purchaseStats = new CardPurchaseStats
            {
                BrainCost = c.BrainCost,
                HeartCost = c.HeartCost,
                SubCost = c.SubCost,

                MinTF = c.MinTF,
                MaxTF = c.MaxTF,

                OfferWeight = 1
            };

            if (c.Class == "Simple")
            {
                return new SimpleCard
                {
                    Name = c.Name,
                    TexturePath = texturePath,
                    PurchaseStats = purchaseStats,

                    BrainGain = c.Brain,
                    HeartGain = c.Heart,
                    SubGain = c.Sub,
                    ShieldGain = c.Shield,
                    Damage = c.Damage,
                    CardDraw = c.Draw
                };
            }

            // Use reflection to create a card of this class
            Type cardClassType = FindCardClass(c.Class);
            if (cardClassType == null)
                throw new NotImplementedException($"No class \"{c.Class}\" class found");

            var card = (ICard)Activator.CreateInstance(cardClassType);
            card.Name = c.Name;
            card.TexturePath = texturePath;
            card.PurchaseStats = purchaseStats;

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

        private class SimpleCardJson
        {
            public string Name {get; set;}
            public string Image {get; set;}
            public string Class {get; set;}

            public int MinTF {get; set;}
            public int MaxTF {get; set;}

            public int BrainCost {get; set;}
            public int HeartCost {get; set;}
            public int SubCost {get; set;}

            public int Brain {get; set;}
            public int Heart {get; set;}
            public int Sub {get; set;}
            public int Damage {get; set;}
            public int Shield {get; set;}
            public int Draw {get; set;}
        }
    }
}