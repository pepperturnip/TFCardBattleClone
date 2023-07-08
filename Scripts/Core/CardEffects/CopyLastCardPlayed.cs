using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class CopyLastCardPlayed : ICardEffect
    {
        public ResourceType Resource;

        public async Task Activate(BattleController battle)
        {
            var card = CardToPlay(battle.State);

            // Do nothing if no copyable card was found
            if (card == null)
                return;

            // TODO: Play a cool animation for resurrecting the card and then
            // playing it

            // Simulate playing the card
            await card.Effect.Activate(battle);
        }

        public string GetDescription(BattleState state)
            => "Copy the last card played this turn";

        public string GetOverriddenImage(BattleState state) => null;

        private Card CardToPlay(BattleState state)
        {
            // Search the "in play" cards for the last copyable card.
            // This will not find cards that were deleted/forgotten, nor will
            // it find cards that were played last turn.
            return ((IEnumerable<Card>)state.CardsPlayedThisTurn)
                .Reverse()
                .FirstOrDefault(c => c.Effect != this);

            // The following is the original game's logic for selecting a card
            // (but translated into C#).
            // Notice the index-out-of-bounds error: "i" can end up being WAY
            // bigger than the in-play pile!  All you need to do is play two
            // "Extra Credit" cards in a row at the start of your turn, and
            // you'll trigger this.
            /*
            if (state.CardsPlayedThisTurn.Count > 1)
            {
                for (int i = 2; i < (state.Deck.Count + state.Discard.Count + state.CardsPlayedThisTurn.Count); i++)
                {
                    var card = state.CardsPlayedThisTurn[state.CardsPlayedThisTurn.Count - i];
                    if (card.Effect != this)
                    {
                        return card;
                    }
                }
            }
            return null;
            */
        }
    }
}