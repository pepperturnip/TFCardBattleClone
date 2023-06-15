using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public static class PlaceholderCards
    {
        public static IEnumerable<ICard> StartingDeck()
        {
            var mysteriousPills = new SimpleCard
            {
                Name = "Mysterious Pills",
                TexturePath = "res://ApolloSevenImages/cardgame/cards/card7.webp",
                Damage = 1
            };

            for (int i = 0; i < 8; i++)
            {
                yield return new TransitioningBasicCard(i);
            }

            yield return mysteriousPills;
            yield return mysteriousPills;
        }

        public static IEnumerable<ICard> AutoGenerateCatalog()
            => ParseJson("res://PlaceholderCards.json");

        public static IEnumerable<ICard> PermanentBuyPile()
            => ParseJson("res://PlaceholderPermanentBuyPile.json");

        private static IEnumerable<ICard> ParseJson(string filePath)
        {
            string rawJson = FileAccess.GetFileAsString(filePath);
            var parsedJson = JsonConvert.DeserializeObject<SimpleCardJson[]>(rawJson);

            return parsedJson.Select(c => new SimpleCard
            {
                Name = c.Name,
                TexturePath = $"res://ApolloSevenImages/cardgame/cards/{c.Image}",

                BrainGain = c.Brain,
                HeartGain = c.Heart,
                SubGain = c.Sub,
                ShieldGain = c.Shield,
                Damage = c.Damage,
                CardDraw = c.Draw,

                PurchaseStats = new CardPurchaseStats
                {
                    BrainCost = c.BrainCost,
                    HeartCost = c.HeartCost,
                    SubCost = c.SubCost,

                    MinTF = c.MinTF,
                    MaxTF = c.MaxTF,

                    OfferWeight = 1
                }
            });
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