using System;
using System.Collections.Generic;

namespace TFCardBattle.Core
{
    /// <summary>
    /// This class contains the actual game rules and logic.
    /// </summary>
    public class BattleController
    {
        public const int MaxHandSize = 6;

        public readonly BattleState State;

        private readonly Random _rng;

        public BattleController(BattleState state, Random rng)
        {
            State = state;
            _rng = rng;
        }

        public void DrawCard()
        {
            // Reshuffle discard pile into the deck if the deck is empty
            if (State.Deck.Count == 0)
            {
                ReshuffleDiscardIntoDeck();

                // If the deck is _still_ empty after shuffling in the discard
                // pile, then we simply fail to draw a card.
                //
                // Since played cards don't go into the discard pile until the
                // end of the current turn, it is theoretically possible for the
                // player to have both an empty deck _and_ an empty discard pile.
                //
                // This is very likely to happen if the player fills their deck
                // with nothing but draw cards(a strategy that's very appealing
                // to smartasses like me).
                if (State.Deck.Count == 0)
                {
                    // TODO: show a message
                    return;
                }
            }

            // Fail to draw if the player's hand is already full.
            // What can I say, man?  Players _really_ like drawing cards.
            if (State.Hand.Count >= MaxHandSize)
            {
                // TODO: Show a message
                return;
            }

            var card = _rng.PickFrom(State.Deck);
            State.Deck.Remove(card);
            State.Hand.Add(card);
        }

        public void ReshuffleDiscardIntoDeck()
        {
            State.Deck.AddRange(State.Discard);
            State.Discard.Clear();
        }
    }
}