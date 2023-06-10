using System;
using System.Collections.Generic;

namespace TFCardBattle.Core
{
    /// <summary>
    /// This class contains the state of a battle.
    /// It does NOT contain any game rules or logic.
    /// </summary>
    public class BattleState
    {
        public int TurnsElapsed = 0;

        public int PlayerTF = 0;
        public int EnemyTF = 0;

        public int PlayerMaxTF = 100;
        public int EnemyMaxTF = 100;

        public int Brain = 0;
        public int Heart = 0;
        public int Sub = 0;
        public int Shield = 0;
        public int Damage = 0;

        public List<ICard> Deck = new List<ICard>();
        public List<ICard> Hand = new List<ICard>();
        public List<ICard> CardsPlayedThisTurn = new List<ICard>();
        public List<ICard> Discard = new List<ICard>();

        /// <summary>
        /// The cards currently on offer to the player this turn.
        /// Gets repopulated with random cards from
        /// <see cref="BattleState.OfferableCards"/> at the start of each turn.
        /// </summary>
        public List<ICard> BuyPile = new List<ICard>();

        /// <summary>
        /// All of the cards that could be offered to the player on any given
        /// turn (ignoring TF levels)
        /// </summary>
        public ICard[] OfferableCards;
    }
}