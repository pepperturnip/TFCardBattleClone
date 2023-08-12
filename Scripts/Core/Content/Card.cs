using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TFCardBattle.Core
{
    public readonly record struct CardId(string Id)
    {
        public static implicit operator CardId(string id) => new CardId(id);
        public static implicit operator string(CardId c) => c.Id;
        public override string ToString() => Id;
    }

    public class Card
    {
        public string Name;
        public string Image;
        public string[] Gifs = new string[] {};

        /// <summary>
        /// Set this to "true" in the JSON to cheat this card into your hand
        /// at the start of every turn.
        /// </summary>
        public bool DebugCheatIntoHand = false;

        public int MinTF;
        public int MaxTF;
        public int OfferWeight = 1;

        public int BrainCost;
        public int HeartCost;
        public int SubCost;
        public int RainbowCost;

        public bool DestroyOnActivate;

        [JsonConverter(typeof(Parsing.CardEffectJsonConverter))]
        public ICardEffect Effect;

        public string GetDescription(BattleState state)
        {
            return Effect.GetDescription(state);
        }

        public string GetImage(BattleState state)
        {
            return Effect.GetOverriddenImage(state) ?? Image;
        }

        public int GetCost(ResourceType resource)
        {
            switch (resource)
            {
                case ResourceType.Brain: return BrainCost;
                case ResourceType.Heart: return HeartCost;
                case ResourceType.Sub: return SubCost;
                default: throw new ArgumentException("Only brain, heart, and sub are accepted");
            }
        }
    }
}