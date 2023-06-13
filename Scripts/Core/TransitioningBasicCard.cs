using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core
{
    /// <summary>
    /// For the most part, ICards are supposed to be immutable.
    /// This card, however, is the one hacky exception.  When basic cards
    /// transition, the TransitionState is mutated in-place.
    ///
    /// Ideally, we'd implement card transitioning by removing the old card
    /// from the player's deck and adding a new one.  But I'm lazy, and this
    /// was easier in the short term.
    ///
    /// I'm not sure what the consequences of this hack are.  But whatever they
    /// are, they're mitigated by the fact that the basic cards are instantiated
    /// brand-new at the start of each game.  If the same ICards were reused
    /// between batlles, then this statefulness would be a concern, because then
    /// we'd need to ensure we reset each card between battles.
    ///
    /// Tl;dr: Assume all ICards are immutable, except this one.
    /// </summary>
    public class TransitioningBasicCard : ICard
    {
        public enum State
        {
            Brain,
            Heart,
            Sub
        }
        public State TransitionState {get; private set;} = State.Brain;
        public readonly int TransitionId;

        public string Name
        {
            get
            {
                switch (TransitionState)
                {
                    case State.Brain: return "Brainstorm";
                    case State.Heart: return "Flirt";
                    case State.Sub: return "Submit";
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public string Desc
        {
            get
            {
                switch (TransitionState)
                {
                    case State.Brain: return "Brain: +1";
                    case State.Heart: return "Heart: + 1";
                    case State.Sub: return "Sub: +1";
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public string TexturePath
        {
            get
            {
                const string prefix = "res://ApolloSevenImages/cardgame/cards";
                switch (TransitionState)
                {
                    case State.Brain: return $"{prefix}/card8.webp";
                    case State.Heart: return $"{prefix}/card9.webp";
                    case State.Sub: return $"{prefix}/card5.webp";
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public CardPurchaseStats PurchaseStats => default;

        public TransitioningBasicCard(int transitionId)
        {
            TransitionId = transitionId;
        }

        public Task Activate(BattleController battle)
        {
            switch (TransitionState)
            {
                case State.Brain: battle.State.Brain++; break;
                case State.Heart: battle.State.Heart++; break;
                case State.Sub: battle.State.Sub++; break;
                default: throw new IndexOutOfRangeException();
            }

            return Task.CompletedTask;
        }

        private const int FlirtBeginTF = 10;
        private const int FlirtHoldTF = 40;
        private const int SubmitBeginTF = 45;
        private const int SubmitHoldTF = 85;
        private const int TotalBasicCards = 8;

        public void UpdateTransitionState(int playerTF)
        {
            TransitionState = TransitionStateAtTF(TransitionId, playerTF);
        }

        private static State TransitionStateAtTF(int transitionId, int playerTF)
        {
            int heartCount = HeartCardsAtTF(playerTF);

            if (playerTF < SubmitBeginTF)
            {
                return (transitionId < heartCount)
                    ? State.Heart
                    : State.Brain;
            }
            else
            {
                return (transitionId < heartCount)
                    ? State.Heart
                    : State.Sub;
            }
        }

        private static int HeartCardsAtTF(int playerTF)
        {
            if (playerTF < FlirtBeginTF)
                return 0;
            else if (playerTF >= FlirtBeginTF && playerTF < FlirtHoldTF)
                return (int)(Math.Ceiling((decimal)(playerTF - FlirtBeginTF) / 4));
            else if (playerTF >= FlirtHoldTF && playerTF < SubmitBeginTF)
                return TotalBasicCards;
            else if (playerTF >= SubmitBeginTF && playerTF < SubmitHoldTF)
                return (int)(Math.Floor((decimal)(TotalBasicCards - ((playerTF - SubmitBeginTF) / 5))));
            else if (playerTF >= SubmitHoldTF)
                return 0;

            throw new Exception("You somehow forgot to cover a case, dude.");
        }
    }
}