using System;
using System.Linq;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardClasses
{
    public class TriumphOfTheMind : ICard
    {
        public string Name { get; set; }
        public string Desc => "Take an extra turn, if you own no heart cards.";
        public string Image { get; set; }
        public string[] Gifs {get; set;}
        public CardPurchaseStats PurchaseStats { get; set; }
        public bool DestroyOnActivate {get; set;}

        public async Task Activate(BattleController battle)
        {
            // TODO: Change the description and texture if the player has heart
            // cards in their deck.

            bool anyHeart = battle.State
                .OwnedCards
                .Any(c => c.PurchaseStats.HeartCost > 0);

            if (!anyHeart)
                await battle.TakeExtraTurn();
        }

        public string GetImage(BattleState state) => Image;
    }
}