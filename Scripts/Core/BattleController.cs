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
        public const int StartingHandSize = 5;

        public const int EnemyMinTFDamage = 0;
        public const int EnemyMaxTFDamage = 5;

        public readonly BattleState State;

        public bool BattleEnded {get; private set;} = false;

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
                TransferAllCards(State.Discard, State.Deck);

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

        public void PlayCard(int handIndex)
        {
            var card = State.Hand[handIndex];

            State.Hand.Remove(card);
            State.PlayedThisTurn.Add(card);
            card.Activate(this);
        }

        public void StartTurn()
        {
            AssertBattleRunning();

            // Throw out the player's unused cards and resources from the last
            // turn, and draw a new hand.
            //
            // Sorry, but this is one of those use-it-or-lose-it card games.
            State.Brain = 0;
            State.Heart = 0;
            State.Subs = 0;
            State.Shield = 0;
            State.TF = 0;

            TransferAllCards(State.PlayedThisTurn, State.Discard);
            TransferAllCards(State.Hand, State.Discard);

            for (int i = 0; i < StartingHandSize; i++)
            {
                DrawCard();
            }

            // TODO: refresh the buy pile
        }

        public void EndTurn()
        {
            // Allow the player to attack
            State.EnemyTF += State.TF;
            if (State.EnemyTF >= State.EnemyMaxTF)
            {
                EndBattle();
                return;
            }

            // Allow the enemy to attack.
            int enemyTfDamage = RollEnemyDamage() - State.Shield;
            enemyTfDamage = Math.Clamp(enemyTfDamage, 0, int.MaxValue);

            State.PlayerTF += enemyTfDamage;
            if (State.PlayerTF >= State.PlayerMaxTF)
            {
                EndBattle();
                return;
            }

            // Start the next turn
            State.TurnsElapsed++;
            StartTurn();
        }

        private void TransferAllCards(List<ICard> src, List<ICard> dst)
        {
            dst.AddRange(src);
            src.Clear();
        }

        private void EndBattle()
        {
            BattleEnded = true;
        }

        private int RollEnemyDamage()
        {
            return _rng.Next(EnemyMinTFDamage, EnemyMaxTFDamage + 1);
        }

        private void AssertBattleRunning()
        {
            if (BattleEnded)
                throw new Exception("You can't do that after the battle has ended");
        }
    }
}