using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core
{
    public class TransitioningBasicCard : ICard
    {
        public enum State
        {
            Brain,
            Heart,
            Sub
        }
        public State TransitionState = State.Brain;
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
    }
}