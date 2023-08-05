using System;
using System.Linq;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class TriumphOfTheMind : ICardEffect
    {
        public async Task Activate(BattleController battle)
        {
            bool anyHeart = battle.State
                .OwnedCards
                .Any(c => c.HeartCost > 0);

            if (!anyHeart)
                await TakeExtraTurn(battle);
        }

        public string GetDescription(BattleState state)
            => "Take an extra turn, if you own no heart cards.";

        // TODO: Change the description and texture if the player has heart
        // cards in their deck.
        public string GetOverriddenImage(BattleState state) => AnyHeart(state)
            ? "res://Media/Cards/card64b.webp"
            : null;

        private bool AnyHeart(BattleState state) => state
            .OwnedCards
            .Any(c => c.HeartCost > 0);

        private async Task TakeExtraTurn(BattleController battle)
        {
            // What does an "extra turn" mean in this context?
            // Well, according to the original game's source code, it means
            // you discard your hand and in-play cards, and then draw a fresh
            // hand.  It also resets your draw-count to 0.
            //
            // Anything else that would normally trigger at the end of a turn,
            // such as enemy traits, body mods, etc. will not activate.
            // You also don't lose any of your resources, nor do you deal any
            // of the TF damage you've built up.  Nor does it reset the buy pile,
            // either.
            // It literally just moves cards around and nothing else.
            //
            // It also seems to not trigger enemy traits that damage you when
            // you draw X-many cards in one turn.  That's because the original
            // game doesn't count this as "drawing" for those purposes.

            var state = battle.State;

            state.InPlay.TransferAllTo(state.Discard);
            state.Hand.TransferAllTo(state.Discard);
            await battle.AnimationPlayer.DiscardHand();

            for (int i = 0; i < BattleController.StartingHandSize; i++)
            {
                await battle.DrawCard();
            }
            // Fun fact: the original game shuffles your deck _after_ you draw
            // 5, instead of before.  In my opinion, that sounds like a bug, so
            // we're not doing that here.  Instead, we let DrawCard() do the
            // shuffling when the deck runs out.
            state.DrawCount = 0;
        }
    }
}