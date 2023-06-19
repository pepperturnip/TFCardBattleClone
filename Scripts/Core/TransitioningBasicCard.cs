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

        public string Name {get; set;}
        public string Desc {get; private set;}
        public string TexturePath {get; set;}
        public bool DestroyOnActivate => false;
        public CardPurchaseStats PurchaseStats {get; set;}

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

        public string GetTexturePath(BattleState state) => TexturePath;

        // Where did I get these constants from?  By experimenting with the
        // original TF Card Battle game.  I went into sandbox mode, manipulated
        // my TF, counted the number each basic card, and put it all into a
        // spreadsheet, hoping that I could deduce a nice, simple formula to
        // determine when cards transition.
        //
        // Unfortunately, the pattern is _almost_ nice, but there are some
        // weird edge cases.  For example: in the original game, only one card
        // can transition per turn, even if your TF jumps from 0 to 99 in one
        // turn..._usually_.  Sometimes, more than one _will_ transition, and I
        // don't have a goddamn clue why.  And sometimes, _no_ cards will
        // transition, but then if you wait another turn at the same TF level,
        // one of them _will_ suddenly transition, even though your TF level
        // didn't change.
        //
        // Is there a bug in the original game?  Is this just a flaw in my
        // experimental methodology?  Was I just too horny to notice an obvious
        // pattern?  Probably all three!  But regardless, I decided to NOT try
        // to match the original game's behavior exactly, and instead make my
        // OWN rules, with blackjack and hookers!  These rules produce _similar_
        // behavior to the original game, but with some tiny differences in some
        // edge cases.  The most notable difference is that submits won't start
        // appearing until TF level 45, instead of TF level 40.  A small price
        // to pay for mathematical elegance.
        //
        // The rules I've implemented are as follows:
        // * Below 10 TF, all basic cards are brain
        // * Starting at 10 TF, one brain card becomes heart every 4 TF
        // * Between 40 and 45 TF, all basic cards are heart
        // * Starting at 45 TF, one heart card becomes sub every 5 TF
        //      * No, that 5 is NOT a typo.  Hearts DO turn into submits slightly
        //          slower than brains turn into hearts.
        private const int FlirtBeginTF = 10;
        private const int FlirtHoldTF = 40;
        private const int SubmitBeginTF = 45;
        private const int SubmitHoldTF = 85;
        private const int TotalBasicCards = 8;

        public void UpdateTransitionState(int playerTF)
        {
            TransitionState = TransitionStateAtTF(TransitionId, playerTF);

            const string prefix = "res://ApolloSevenImages/cardgame/cards";
            switch (TransitionState)
            {
                case State.Brain:
                {
                    Name = "Brainstorm";
                    Desc = "Brain: +1";
                    TexturePath = $"{prefix}/card8.webp";
                    break;
                }

                case State.Heart:
                {
                    Name = "Flirt";
                    Desc = "Heart: +1";
                    TexturePath = $"{prefix}/card9.webp";
                    break;
                }

                case State.Sub:
                {
                    Name = "Submit";
                    Desc = "Sub: +1";
                    TexturePath = $"{prefix}/card5.webp";
                    break;
                }
            }
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