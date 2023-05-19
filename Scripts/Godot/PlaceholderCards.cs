using System;
using System.Collections.Generic;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public static class PlaceholderCards
    {
        public static IEnumerable<ICard> AutoGenerateCatalog()
        {
            yield return BrainCard("Quick Thinking", brainCost: 3, tf: 1, shield: 1, brain: 1);
            yield return BrainCard("Spreadsheet Master", brainCost: 2, brain: 1, shield: 1);
            yield return BrainCard("Motivational Poster", brainCost: 3, brain: 1, tf: 2);
            yield return BrainCard("Phone a Friend", brainCost: 3, brain: 1, draw: 1);
            yield return BrainCard("Firewall", brainCost: 3, shield: 3);
            yield return BrainCard("Hypnotic Email", brainCost: 5, tf: 4);
            yield return BrainCard("Paranoia", brainCost: 3, shield: 1, draw: 1);
            yield return BrainCard("L33t Hacker", brainCost: 4, shield: 2, brain: 2);
            yield return BrainCard("Eureka", brainCost: 7, brain: 4);
            yield return BrainCard("Counter Measures", brainCost: 4, tf: 2, shield: 2);
            yield return BrainCard("Evil Therapist", brainCost: 7, tf: 3, draw: 2);
            yield return BrainCard("Research Lab(sorta)", brainCost: 6, draw: 2);
            yield return BrainCard("All Nighter", brainCost: 6, draw: 2);

            yield return HeartCard("Second Base", heartCost: 1, heart: 2);
            yield return HeartCard("Cam Girl", heartCost: 3, heart: 2, tf: 1, shield: 1);
            yield return HeartCard("Beauty Regimen", heartCost: 2, shield: 1, draw: 1);
            yield return HeartCard("Just a Lick", heartCost: 2, tf: 1, draw: 1);
            yield return HeartCard("Bribe the Intern", heartCost: 1, tf: 2);
            yield return HeartCard("Crotch Grab", heartCost: 3, heart: 3);
            yield return HeartCard("Date Night", heartCost: 3, heart: 2, shield: 2);
            yield return HeartCard("Sixty-Nine", heartCost: 4, tf: 3, heart: 3);
            yield return HeartCard("Suggestive Moves", heartCost: 2, heart: 1, draw: 1);
            yield return HeartCard("Strapon", heartCost: 3, heart: 2, brain: 2);
            yield return HeartCard("Seduce the Cable Guy", heartCost: 3, tf: 2, draw: 1);
            yield return HeartCard("Back to His Place", heartCost: 3, shield: 3, tf: 2);
            yield return HeartCard("Sugar Daddy", heartCost: 2, shield: 3);

            yield return SubCard("Bow Down", subCost: 1, tf: 1, sub: 2);
            yield return SubCard("Ropeplay", heartCost: 2, sub: 3);
            yield return SubCard("Facial", heartCost: 2, heart: 2, sub: 2);
            yield return SubCard("Anal", heartCost: 5, heart: 3, draw: 1);
            yield return SubCard("Underfoot", subCost: 1, tf: 2, shield: 1);
            yield return SubCard("Blindfolded", heartCost: 2, sub: 1, draw: 1);
            yield return SubCard("Creampie", heartCost: 7, sub: 5, draw: 2);
            yield return SubCard("Pounded", heartCost: 5, tf: 2, sub: 2, draw: 1);
            yield return SubCard("At Master's Feet", subCost: 2, sub: 2, draw: 1);
            yield return SubCard("Locked Away", subCost: 2, shield: 3, sub: 1);
            yield return SubCard("Swallow(sorta)", heartCost: 8, draw: 3);
            yield return SubCard("Air Tight", subCost: 5, tf: 3, shield: 3, sub: 3);
            yield return SubCard("Ride 'em Cowgirl", heartCost: 6, tf: 6, draw: 1);
            yield return SubCard("Nipple Clamps", subCost: 2, sub: 3, heart: 2);
            yield return SubCard("Lick it Up", subCost: 6, draw: 3);
            yield return SubCard("Blacked", heartCost: 5, draw: 2);
            yield return SubCard("Gangbang", subCost: 5, tf: 10);
            yield return SubCard("Inspection", subCost: 4, draw: 2);
            yield return SubCard("Branded", subCost: 5, shield: 5, draw: 1);
        }

        private static ICard BrainCard(
            string name,
            int brainCost = 0,
            int heartCost = 0,
            int subCost = 0,
            int brain = 0,
            int heart = 0,
            int sub = 0,
            int tf = 0,
            int shield = 0,
            int draw = 0
        )
        {
            return new SimpleCard
            {
                Name = name,
                PurchaseStats = new CardPurchaseStats
                {
                    BrainCost = brainCost,
                    HeartCost = heartCost,
                    SubsCost = subCost,
                    MinTF = 0,
                    MaxTF = 42,
                    OfferWeight = 1
                },

                BrainGain = brain,
                HeartGain = heart,
                SubsGain = sub,
                ShieldGain = shield,
                TFGain = tf,
                CardDraw = draw
            };
        }

        private static ICard HeartCard(
            string name,
            int brainCost = 0,
            int heartCost = 0,
            int subCost = 0,
            int brain = 0,
            int heart = 0,
            int sub = 0,
            int tf = 0,
            int shield = 0,
            int draw = 0
        )
        {
            return new SimpleCard
            {
                Name = name,
                PurchaseStats = new CardPurchaseStats
                {
                    BrainCost = brainCost,
                    HeartCost = heartCost,
                    SubsCost = subCost,
                    MinTF = 33,
                    MaxTF = 71,
                    OfferWeight = 1
                },

                BrainGain = brain,
                HeartGain = heart,
                SubsGain = sub,
                ShieldGain = shield,
                TFGain = tf,
                CardDraw = draw
            };
        }

        private static ICard SubCard(
            string name,
            int brainCost = 0,
            int heartCost = 0,
            int subCost = 0,
            int brain = 0,
            int heart = 0,
            int sub = 0,
            int tf = 0,
            int shield = 0,
            int draw = 0
        )
        {
            return new SimpleCard
            {
                Name = name,
                PurchaseStats = new CardPurchaseStats
                {
                    BrainCost = brainCost,
                    HeartCost = heartCost,
                    SubsCost = subCost,
                    MinTF = 66,
                    MaxTF = int.MaxValue,
                    OfferWeight = 1
                },

                BrainGain = brain,
                HeartGain = heart,
                SubsGain = sub,
                ShieldGain = shield,
                TFGain = tf,
                CardDraw = draw
            };
        }
    }
}