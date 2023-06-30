using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class Outwit : ICardEffect
    {
        public Task Activate(BattleController battle)
        {
            var oldBuyPile = battle.State.BuyPile;
            var permanentBuyPileCard = oldBuyPile[oldBuyPile.Count - 1];

            // Offer a bunch of brain cards
            var offerableCards = OfferableCards(battle.State).ToHashSet();
            var buyPile = new HashSet<Card>();

            while(buyPile.Count < BattleController.OfferedCardCount && offerableCards.Count > 0)
            {
                var weights = offerableCards
                    .Select(c => (c, c.OfferWeight))
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

        public string GetDescription(BattleState state)
            => "Change the buy piles to random brain cards";

        public string GetOverriddenImage(BattleState state) => null;

        private IEnumerable<Card> OfferableCards(BattleState state)
        {
            return state.PlayerLoadout
                .OfferableCards
                .Where(c => c.BrainCost > 0)
                .Where(c => c.HeartCost <= 0)
                .Where(c => c.SubCost <= 0);
        }
    }
}