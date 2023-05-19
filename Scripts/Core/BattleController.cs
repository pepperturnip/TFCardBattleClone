using System;
using System.Collections.Generic;

namespace TFCardBattle.Core
{
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
            // TODO: Reshuffle discard pile into the deck if the deck is empty
            // TODO: Show "deck is empty" message if deck and discard both empty
            if (State.Deck.Count == 0)
                return;

            // TODO: Show a "hand is full" message
            if (State.Hand.Count >= MaxHandSize)
                return;

            var card = _rng.PickFrom(State.Deck);
            State.Deck.Remove(card);
            State.Hand.Add(card);
        }
    }
}