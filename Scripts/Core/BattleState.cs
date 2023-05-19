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
        public int Subs = 0;
        public int Shield = 0;
        public int TF = 0;

        public List<ICard> Deck = new List<ICard>();
        public List<ICard> Hand = new List<ICard>();
        public List<ICard> CardsPlayedThisTurn = new List<ICard>();
        public List<ICard> Discard = new List<ICard>();

        public List<ICard> BuyPile = new List<ICard>();
    }
}