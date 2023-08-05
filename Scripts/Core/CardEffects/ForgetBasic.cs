using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class ForgetBasic : ICardEffect
    {
        public async Task Activate(BattleController battle)
        {
            var state = battle.State;

            if (await TryForgetFrom(state.Deck))
                return;

            if (await TryForgetFrom(state.Discard))
                return;

            if (await TryForgetFrom(state.InPlay))
                return;

            if (await TryForgetFrom(state.Hand))
                return;

            // TODO: Show some message saying no card could be forgotten

            bool IsBasic(Card c)
            {
                // TODO: Allow Mysterious Pills to be deleted as well
                return c.Effect is CardEffects.TransitioningBasicCard;
            }

            async Task<bool> TryForgetFrom(List<Card> cards)
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    var card = cards[i];

                    if (IsBasic(card))
                    {
                        cards.RemoveAt(i);
                        await battle.AnimationPlayer.ForgetCard(card, state);
                        return true;
                    }
                }

                return false;
            }
        }

        public string GetDescription(BattleState state)
            => "Remove a basic card from your deck.";

        public string GetOverriddenImage(BattleState state) => null;
    }
}