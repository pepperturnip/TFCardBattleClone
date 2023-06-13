using System;
using System.Collections.Generic;
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
        {
            yield return BrainCard("Quick Thinking", "card10.webp", brainCost: 3, damage: 1, shield: 1, brain: 1);
            yield return BrainCard("Spreadsheet Master", "card18.webp", brainCost: 2, brain: 1, shield: 1);
            yield return BrainCard("Motivational Poster", "card12.webp", brainCost: 3, brain: 1, damage: 2);
            yield return BrainCard("Phone a Friend", "card17.webp", brainCost: 3, brain: 1, draw: 1);
            yield return BrainCard("Firewall", "card15.webp", brainCost: 3, shield: 3);
            yield return BrainCard("Hypnotic Email", "card13.webp", brainCost: 5, damage: 4);
            yield return BrainCard("Paranoia", "card20.webp", brainCost: 3, shield: 1, draw: 1);
            yield return BrainCard("L33t Hacker", "card28.webp", brainCost: 4, shield: 2, brain: 2);
            yield return BrainCard("Eureka", "card33.webp", brainCost: 7, brain: 4);
            yield return BrainCard("Counter Measures", "card21.webp", brainCost: 4, damage: 2, shield: 2);
            yield return BrainCard("Evil Therapist", "card45.webp", brainCost: 7, damage: 3, draw: 2);
            yield return BrainCard("Research Lab(sorta)", "card51.webp", brainCost: 6, draw: 2);
            yield return BrainCard("All Nighter", "card19.webp", brainCost: 6, draw: 2);

            yield return HeartCard("Second Base", "card29.webp", heartCost: 1, heart: 2);
            yield return HeartCard("Cam Girl", "card61.webp", heartCost: 3, heart: 2, damage: 1, shield: 1);
            yield return HeartCard("Beauty Regimen", "card32.webp", heartCost: 2, shield: 1, draw: 1);
            yield return HeartCard("Just a Lick", "card52.webp", heartCost: 2, damage: 1, draw: 1);
            yield return HeartCard("Bribe the Intern", "card30.webp", heartCost: 1, damage: 2);
            yield return HeartCard("Crotch Grab", "card41.webp", heartCost: 3, heart: 3);
            yield return HeartCard("Date Night", "card40.webp", heartCost: 3, heart: 2, shield: 2);
            yield return HeartCard("Sixty-Nine", "card60.webp", heartCost: 4, damage: 3, heart: 3);
            yield return HeartCard("Suggestive Moves", "card35.webp", heartCost: 2, heart: 1, draw: 1);
            yield return HeartCard("Strapon", "card48.webp", heartCost: 3, heart: 2, brain: 2);
            yield return HeartCard("Seduce the Cable Guy", "card44.webp", heartCost: 3, damage: 2, draw: 1);
            yield return HeartCard("Back to His Place", "card50.webp", heartCost: 3, shield: 3, damage: 2);
            yield return HeartCard("Sugar Daddy", "card57.webp", heartCost: 2, shield: 3);

            yield return SubCard("Bow Down", "card71.webp", subCost: 1, damage: 1, sub: 2);
            yield return SubCard("Ropeplay", "card72.webp", heartCost: 2, sub: 3);
            yield return SubCard("Facial", "card67.webp", heartCost: 2, heart: 2, sub: 2);
            yield return SubCard("Anal", "card77.webp", heartCost: 5, heart: 3, draw: 1);
            yield return SubCard("Underfoot", "card73.webp", subCost: 1, damage: 2, shield: 1);
            yield return SubCard("Blindfolded", "card74.webp", heartCost: 2, sub: 1, draw: 1);
            yield return SubCard("Creampie", "card80.webp", heartCost: 7, sub: 5, draw: 2);
            yield return SubCard("Pounded", "card89.webp", heartCost: 5, damage: 2, sub: 2, draw: 1);
            yield return SubCard("At Master's Feet", "card91.webp", subCost: 2, sub: 2, draw: 1);
            yield return SubCard("Locked Away", "card76.webp", subCost: 2, shield: 3, sub: 1);
            yield return SubCard("Swallow(sorta)", "card83.webp", heartCost: 8, draw: 3);
            yield return SubCard("Air Tight", "card69.webp", subCost: 5, damage: 3, shield: 3, sub: 3);
            yield return SubCard("Ride 'em Cowgirl", heartCost: 6, damage: 6, draw: 1);
            yield return SubCard("Nipple Clamps", "card87.webp", subCost: 2, sub: 3, heart: 2);
            yield return SubCard("Lick it Up", "card95.webp", subCost: 6, draw: 3);
            yield return SubCard("Blacked", "card84.webp", heartCost: 5, draw: 2);
            yield return SubCard("Gangbang", "card93.webp", subCost: 5, damage: 10);
            yield return SubCard("Inspection", "card75.webp", subCost: 4, draw: 2);
            yield return SubCard("Branded", "card94.webp", subCost: 5, shield: 5, draw: 1);
        }

        private static ICard BrainCard(
            string name,
            string fileName = "",
            int brainCost = 0,
            int heartCost = 0,
            int subCost = 0,
            int brain = 0,
            int heart = 0,
            int sub = 0,
            int damage = 0,
            int shield = 0,
            int draw = 0
        )
        {
            return new SimpleCard
            {
                Name = name,
                TexturePath = $"res://ApolloSevenImages/cardgame/cards/{fileName}",
                PurchaseStats = new CardPurchaseStats
                {
                    BrainCost = brainCost,
                    HeartCost = heartCost,
                    SubCost = subCost,
                    MinTF = 0,
                    MaxTF = 42,
                    OfferWeight = 1
                },

                BrainGain = brain,
                HeartGain = heart,
                SubGain = sub,
                ShieldGain = shield,
                Damage = damage,
                CardDraw = draw
            };
        }

        private static ICard HeartCard(
            string name,
            string fileName = "",
            int brainCost = 0,
            int heartCost = 0,
            int subCost = 0,
            int brain = 0,
            int heart = 0,
            int sub = 0,
            int damage = 0,
            int shield = 0,
            int draw = 0
        )
        {
            return new SimpleCard
            {
                Name = name,
                TexturePath = $"res://ApolloSevenImages/cardgame/cards/{fileName}",
                PurchaseStats = new CardPurchaseStats
                {
                    BrainCost = brainCost,
                    HeartCost = heartCost,
                    SubCost = subCost,
                    MinTF = 33,
                    MaxTF = 71,
                    OfferWeight = 1
                },

                BrainGain = brain,
                HeartGain = heart,
                SubGain = sub,
                ShieldGain = shield,
                Damage = damage,
                CardDraw = draw
            };
        }

        private static ICard SubCard(
            string name,
            string fileName = "",
            int brainCost = 0,
            int heartCost = 0,
            int subCost = 0,
            int brain = 0,
            int heart = 0,
            int sub = 0,
            int damage = 0,
            int shield = 0,
            int draw = 0
        )
        {
            return new SimpleCard
            {
                Name = name,
                TexturePath = $"res://ApolloSevenImages/cardgame/cards/{fileName}",
                PurchaseStats = new CardPurchaseStats
                {
                    BrainCost = brainCost,
                    HeartCost = heartCost,
                    SubCost = subCost,
                    MinTF = 66,
                    MaxTF = int.MaxValue,
                    OfferWeight = 1
                },

                BrainGain = brain,
                HeartGain = heart,
                SubGain = sub,
                ShieldGain = shield,
                Damage = damage,
                CardDraw = draw
            };
        }
    }
}