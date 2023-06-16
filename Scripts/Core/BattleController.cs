using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TFCardBattle.Core
{
    /// <summary>
    /// This class contains the actual game rules and logic.
    /// </summary>
    public class BattleController
    {
        public const int MaxHandSize = 6;
        public const int StartingHandSize = 5;
        public const int OfferedCardCount = 4;

        public const int EnemyMinTFDamage = 0;
        public const int EnemyMaxTFDamage = 5;

        public readonly BattleState State;

        public bool BattleEnded {get; private set;} = false;

        private readonly IBattleAnimationPlayer _animationPlayer;
        private readonly Random _rng;

        public BattleController(
            PlayerLoadout loadout,
            IBattleAnimationPlayer animationPlayer,
            Random rng
        )
        {
            State = new BattleState(loadout);
            _animationPlayer = animationPlayer;
            _rng = rng;
        }

        public async Task DrawCard()
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

            await _animationPlayer.DrawCard(card);
        }

        public async Task PlayCard(int handIndex)
        {
            var card = State.Hand[handIndex];

            State.Hand.Remove(card);
            State.CardsPlayedThisTurn.Add(card);
            await _animationPlayer.PlayCard(handIndex, State);

            await card.Activate(this);
        }

        public async Task BuyCard(int buyPileIndex)
        {
            // Fail to buy the card if the player can't afford it.
            // TODO: Show some kind of message
            if (!CanAffordCard(buyPileIndex))
                return;

            bool isPermanentCard = IsPermanentBuyPileCard(buyPileIndex);

            var card = State.BuyPile[buyPileIndex];

            if (!isPermanentCard)
                State.BuyPile.RemoveAt(buyPileIndex);

            State.Discard.Add(card);

            var cost = card.PurchaseStats;
            State.Brain -= cost.BrainCost;
            State.Heart -= cost.HeartCost;
            State.Sub -= cost.SubCost;

            await _animationPlayer.BuyCard(buyPileIndex, isPermanentCard);
        }

        public async Task StartTurn()
        {
            AssertBattleRunning();
            TransitionBasicCards();

            await RefreshBuyPile();

            // Throw out the player's unused cards and resources from the last
            // turn, and draw a new hand.
            //
            // Sorry, but this is one of those use-it-or-lose-it card games.
            State.Brain = 0;
            State.Heart = 0;
            State.Sub = 0;
            State.Shield = 0;
            State.Damage = 0;

            TransferAllCards(State.CardsPlayedThisTurn, State.Discard);
            TransferAllCards(State.Hand, State.Discard);
            await _animationPlayer.DiscardHand();

            for (int i = 0; i < StartingHandSize; i++)
            {
                await DrawCard();
            }
        }

        public async Task EndTurn()
        {
            // Allow the player to attack
            State.EnemyTF += State.Damage;
            await _animationPlayer.DamageEnemy(State.Damage);

            if (State.EnemyTF >= State.EnemyMaxTF)
            {
                EndBattle();
                return;
            }

            // Allow the enemy to attack.
            int enemyTfDamage = RollEnemyDamage() - State.Shield;
            enemyTfDamage = Math.Clamp(enemyTfDamage, 0, int.MaxValue);

            State.PlayerTF += enemyTfDamage;
            await _animationPlayer.DamagePlayer(enemyTfDamage);

            if (State.PlayerTF >= State.PlayerMaxTF)
            {
                EndBattle();
                return;
            }

            // Start the next turn
            State.TurnsElapsed++;
            await StartTurn();
        }

        public Task RefreshBuyPile()
        {
            // Offer a random set of cards to the player
            var offerableCards = CardsOfferableAtTf(
                State.PlayerLoadout.OfferableCards,
                State.PlayerTF
            ).ToHashSet();

            var buyPile = new HashSet<ICard>();

            while(buyPile.Count < OfferedCardCount && offerableCards.Count > 0)
            {
                var weights = offerableCards
                    .Select(c => (c, c.PurchaseStats.OfferWeight))
                    .ToArray();

                var card = _rng.PickFromWeighted(weights);
                offerableCards.Remove(card);
                buyPile.Add(card);
            }

            State.BuyPile = buyPile.ToList();

            // Add a non-random, "permanent" card to the buy pile, based on the
            // player's TF.
            var permanentCard = CardsOfferableAtTf(
                State.PlayerLoadout.PermanentBuyPile,
                State.PlayerTF
            ).Single();
            State.BuyPile.Add(permanentCard);

            return _animationPlayer.RefreshBuyPile(State.BuyPile.ToArray());
        }

        public async Task ForgetBasicCard()
        {
            if (await TryForgetFrom(State.Deck))
            return;

            if (await TryForgetFrom(State.Discard))
                return;

            if (await TryForgetFrom(State.CardsPlayedThisTurn))
                return;

            if (await TryForgetFrom(State.Hand))
                return;

            // TODO: Show some message saying no card could be forgotten

            bool IsBasic(ICard c)
            {
                // TODO: Allow Mysterious Pills to be deleted as well
                return c is TransitioningBasicCard;
            }

            async Task<bool> TryForgetFrom(List<ICard> cards)
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    var card = cards[i];

                    if (IsBasic(card))
                    {
                        cards.RemoveAt(i);
                        await _animationPlayer.ForgetCard(card);
                        return true;
                    }
                }

                return false;
            }
        }

        public bool CanAffordCard(int buyPileIndex)
        {
            var card = State.BuyPile[buyPileIndex].PurchaseStats;

            if (State.Brain < card.BrainCost)
                return false;

            if (State.Heart < card.HeartCost)
                return false;

            if (State.Sub < card.SubCost)
                return false;

            return true;
        }

        private bool IsPermanentBuyPileCard(int buyPileIndex)
        {
            return buyPileIndex == State.BuyPile.Count - 1;
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

        private void TransitionBasicCards()
        {
            var allCards = State.Deck
                .Concat(State.Hand)
                .Concat(State.Discard)
                .Concat(State.CardsPlayedThisTurn);

            foreach (var card in allCards)
            {
                if (card is TransitioningBasicCard transCard)
                {
                    transCard.UpdateTransitionState(State.PlayerTF);
                }
            }
        }

        private void AssertBattleRunning()
        {
            if (BattleEnded)
                throw new Exception("You can't do that after the battle has ended");
        }

        private IEnumerable<ICard> CardsOfferableAtTf(IEnumerable<ICard> cards, int tf)
        {
            return cards
                .Where(c => tf <= c.PurchaseStats.MaxTF)
                .Where(c => tf >= c.PurchaseStats.MinTF);
        }
    }
}