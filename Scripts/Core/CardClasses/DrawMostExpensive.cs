using System;
using System.Threading.Tasks;
using System.Linq;

namespace TFCardBattle.Core.CardClasses
{
    public class DrawMostExpensive : ICard
    {
        public string Name {get; set;}
        public string Desc => $"Draw the highest-cost {Resource} card from your deck";
        public string TexturePath {get; set;}
        public CardPurchaseStats PurchaseStats {get; set;}
        public bool DestroyOnActivate {get; set;}

        public ResourceType Resource {get; set;}

        public Task Activate(BattleController battle)
        {
            battle.State.DrawCount++;

            ICard card = battle.State.Deck
                .Where(c => c.PurchaseStats.HeartCost > 0)
                .OrderByDescending(c => c.PurchaseStats.HeartCost)
                .FirstOrDefault();

            if (card == null)
            {
                // TODO: give the player "consolation prize" instead of adding 1
                // heart
                battle.State.Heart++;
                return Task.CompletedTask;
            }

            battle.State.Hand.Add(card);
            battle.State.Deck.Remove(card);
            return battle.AnimationPlayer.DrawCard(card);
        }

        public string GetTexturePath(BattleState state) => TexturePath;
    }
}