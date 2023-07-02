using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TFCardBattle.Core
{
    public class Card
    {
        public string Name;
        public string Image;
        public string[] Gifs = new string[] {};

        public int MinTF;
        public int MaxTF;
        public int OfferWeight = 1;

        public int BrainCost;
        public int HeartCost;
        public int SubCost;

        public bool DestroyOnActivate;

        [JsonIgnore] public ICardEffect Effect;

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

    public interface ICardEffect
    {
        Task Activate(BattleController battle);

        string GetDescription(BattleState state);
        string GetOverriddenImage(BattleState state);
    }
}