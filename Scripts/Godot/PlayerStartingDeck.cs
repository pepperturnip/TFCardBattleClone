using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public static class PlayerStartingDeck
    {
        public static IEnumerable<Card> StartingDeck()
        {
            var mysteriousPills = new Card
            {
                Name = "Mysterious Pills",
                Image = "res://Media/Cards/card7.webp",
                Effect = new Core.CardEffects.Simple
                {
                    Damage = 1
                }
            };

            for (int i = 0; i < 8; i++)
            {
                yield return new Card
                {
                    Name = "Basic Card",
                    Effect = new Core.CardEffects.TransitioningBasicCard(i)
                };
            }

            yield return mysteriousPills;
            yield return mysteriousPills;
        }
    }
}