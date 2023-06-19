using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardClasses
{
    public class Outwit : ICard
    {
        public string Name {get; set;}

        public string Desc => "Change the buy piles to random brain cards";

        public string TexturePath {get; set;}

        public CardPurchaseStats PurchaseStats {get; set;}

        public Task Activate(BattleController battle)
        {
            var oldBuyPile = battle.State.BuyPile;
            var permanentBuyPileCard = oldBuyPile[oldBuyPile.Count - 1];

            // Offer a bunch of brain cards
            var offerableCards = OfferableCards(battle.State).ToHashSet();
            var buyPile = new HashSet<ICard>();

            while(buyPile.Count < BattleController.OfferedCardCount && offerableCards.Count > 0)
            {
                var weights = offerableCards
                    .Select(c => (c, c.PurchaseStats.OfferWeight))
                    .ToArray();

                var card = battle.Rng.PickFromWeighted(weights);
                offerableCards.Remove(card);
                buyPile.Add(card);
            }

            battle.State.BuyPile = buyPile.ToList();

            // Keep the same permanent buy pile card as before
            battle.State.BuyPile.Add(permanentBuyPileCard);

            // And of course, we can't forget the animation
            return battle.AnimationPlayer.RefreshBuyPile(battle.State.BuyPile.ToArray());
        }

        public string GetTexturePath(BattleState state) => TexturePath;

        private IEnumerable<ICard> OfferableCards(BattleState state)
        {
            return state.PlayerLoadout
                .OfferableCards
                .Where(c => c.PurchaseStats.BrainCost > 0)
                .Where(c => c.PurchaseStats.HeartCost <= 0)
                .Where(c => c.PurchaseStats.SubCost <= 0);
        }
    }
}