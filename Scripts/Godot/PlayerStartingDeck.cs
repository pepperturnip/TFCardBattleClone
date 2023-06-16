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
    }
}