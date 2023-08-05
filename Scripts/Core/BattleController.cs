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
        public const int MaxConsumableSlots = 3;

        public int EnemyMinTFDamage => MinEnemyDamageOnTurn(State.TurnsElapsed);
        public int EnemyMaxTFDamage => MaxEnemyDamageOnTurn(State.TurnsElapsed);

        public readonly BattleState State;
        public readonly Random Rng;
        public readonly IBattleAnimationPlayer AnimationPlayer;

        public bool BattleEnded {get; private set;} = false;

        public BattleController(
            PlayerLoadout loadout,
            Random rng,
            ContentRegistry registry,
            IBattleAnimationPlayer animationPlayer
        )
        {
            State = new BattleState(
                loadout: loadout,
                registry: registry
            );

            Rng = rng;
            AnimationPlayer = animationPlayer;
        }

        public void AddEffect(ILingeringEffect effect)
        {
            State.LingeringEffects.Add(effect);
        }

        public void RemoveEffect(ILingeringEffect effect)
        {
            State.LingeringEffects.Remove(effect);
        }

        private async Task TriggerEffects(Func<ILingeringEffect, Task> action)
        {
            foreach (var effect in State.LingeringEffects.ToArray())
                await action(effect);
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

            var card = Rng.PickFrom(State.Deck);
            State.Deck.Remove(card);
            State.Hand.Add(card);
            State.DrawCount++;

            await AnimationPlayer.DrawCard(card);
        }

        public async Task PlayCard(int handIndex)
        {
            var card = State.Hand[handIndex];

            State.Hand.Remove(card);
            State.InPlay.Add(card);
            await AnimationPlayer.PlayCard(handIndex, State);

            await TriggerEffects(e => e.OnCardAboutToActivate(this, card));
            await card.Effect.Activate(this);
            await TriggerEffects(e => e.OnCardFinishedActivating(this, card));

            if (card.DestroyOnActivate)
            {
                State.InPlay.Remove(card);
                await AnimationPlayer.ForgetCard(card, State);
            }
        }

        public async Task ForgetCardInHand(int handIndex)
        {
            var card = State.Hand[handIndex];
            State.Hand.Remove(card);
            await AnimationPlayer.ForgetCard(card, State);
        }

        public async Task BuyCard(int buyPileIndex)
        {
            // This command is not valid during boss fights.  The button for
            // it shouldn't even be visible then.
            if (State.IsBossRound)
                throw new Exception("Can't buy a card during a boss fight");

            // Fail to buy the card if the player can't afford it.
            // TODO: Show some kind of message
            if (!CanAffordCard(buyPileIndex))
                return;

            bool isPermanentCard = IsPermanentBuyPileCard(buyPileIndex);

            var card = State.BuyPile[buyPileIndex];

            if (!isPermanentCard)
                State.BuyPile.RemoveAt(buyPileIndex);

            State.Discard.Add(card);

            if (card.RainbowCost == 0)
            {
                State.Brain -= card.BrainCost;
                State.Heart -= card.HeartCost;
                State.Sub -= card.SubCost;
            }
            else
            {
                // "Rainbow" cards can be paid for with any combination of
                // resources.  IE: something that costs 15 rainbow can be bought
                // with 15 brain, or with 10 brain + 5 heart, or with 5 of each,
                // etc.
                //
                // So, which resources get spent "first"?
                // It's subs, then hearts, then brains.
                int leftToSpend = card.RainbowCost;
                while (leftToSpend > 0)
                {
                    if (State.Sub > 0)
                        State.Sub--;

                    else if (State.Heart > 0)
                        State.Heart--;

                    else
                        State.Brain--;

                    leftToSpend--;
                }
                // Now, I know what you're thinking.
                // "What the hell, dude?!  Why did you use a loop for this
                // instead of doing some simple math?!"
                //
                // The answer is that I'm both stupid and lazy.
                // I don't know the simple math off the top of my head, and I'm
                // too lazy to figure it.  So instead, I'm just subtracting
                // stuff one-by-one.
            }

            await AnimationPlayer.BuyCard(buyPileIndex, isPermanentCard);
        }

        public Task AddConsumable(Consumable consumable)
        {
            if (State.Consumables.Count >= MaxConsumableSlots)
            {
                // TODO: Show a message saying the consumable slots are full
                return Task.CompletedTask;
            }
            State.Consumables.Add(consumable);

            return Task.CompletedTask;
        }

        public async Task UseConsumable(int consumableIndex)
        {
            var consumable = State.Consumables[consumableIndex];

            State.Consumables.Remove(consumable);
            // TODO: Play an animation of using the consumable

            await TriggerEffects(e => e.OnConsumableAboutToActivate(this, consumable));
            await consumable.Effect.Activate(this);
            await TriggerEffects(e => e.OnConsumableFinishedActivating(this, consumable));
        }

        public async Task StartTurn()
        {
            AssertBattleRunning();

            await DiscardResources();
            await ResetBuyPile();
            await DebugCheatCardsIntoHand();

            // Draw a fresh hand of cards
            for (int i = 0; i < StartingHandSize; i++)
            {
                await DrawCard();
            }
            State.DrawCount = 0;
        }

        public Task EndTurn()
        {
            if (!State.IsBossRound)
                return EndTurnNormal();
            else
                return EndTurnBoss();
        }

        public Task ResetBuyPile()
        {
            // Offer a random set of cards to the player
            var offerableCards = CardsOfferableAtTf(
                State.PlayerLoadout.OfferableCards,
                State.PlayerTF
            ).ToHashSet();

            var buyPile = new HashSet<Card>();

            while(buyPile.Count < OfferedCardCount && offerableCards.Count > 0)
            {
                var weights = offerableCards
                    .Select(c => (c, c.OfferWeight))
                    .ToArray();

                var card = Rng.PickFromWeighted(weights);
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

            return AnimationPlayer.RefreshBuyPile(State.BuyPile.ToArray());
        }

        public async Task DiscardHand()
        {
            TransferAllCards(State.InPlay, State.Discard);
            TransferAllCards(State.Hand, State.Discard);
            await AnimationPlayer.DiscardHand();
        }

        public async Task DiscardResources()
        {
            State.Brain = 0;
            State.Heart = 0;
            State.Sub = 0;
            State.Shield = 0;
            State.Damage = 0;
            await AnimationPlayer.DiscardResources();
            await TriggerEffects(e => e.OnResourcesDiscarded(this));
        }

        public async Task ForgetBasicCard()
        {
            if (await TryForgetFrom(State.Deck))
                return;

            if (await TryForgetFrom(State.Discard))
                return;

            if (await TryForgetFrom(State.InPlay))
                return;

            if (await TryForgetFrom(State.Hand))
                return;

            // TODO: Show some message saying no card could be forgotten

            bool IsBasic(Card c)
            {
                // TODO: Allow Mysterious Pills to be deleted as well
                return c.Effect is CardEffects.TransitioningBasicCard;
            }

            async Task<bool> TryForgetFrom(List<Card> cards)
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    var card = cards[i];

                    if (IsBasic(card))
                    {
                        cards.RemoveAt(i);
                        await AnimationPlayer.ForgetCard(card, State);
                        return true;
                    }
                }

                return false;
            }
        }

        public async Task TakeExtraTurn()
        {
            // For the Triumph of the Mind card.
            //
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
            TransferAllCards(State.InPlay, State.Discard);
            TransferAllCards(State.Hand, State.Discard);
            await AnimationPlayer.DiscardHand();

            for (int i = 0; i < StartingHandSize; i++)
            {
                await DrawCard();
            }
            // Fun fact: the original game shuffles your deck _after_ you draw
            // 5, instead of before.  In my opinion, that sounds like a bug, so
            // we're not doing that here.  Instead, we let DrawCard() do the
            // shuffling when the deck runs out.
            State.DrawCount = 0;

            // TODO: Maybe find a way to reuse the code from StartTurn()?
            // Not sure if that's a good idea yet or not.
        }

        public int MinEnemyDamageOnTurn(int turnZeroBased)
        {
            return ((turnZeroBased - 1) / 12) + 2;
        }

        public int MaxEnemyDamageOnTurn(int turnZeroBased)
        {
            return ((turnZeroBased - 1)) / 6 + 3;
        }

        public int TotalDamageToBoss()
        {
            // During the boss round, the three main resources count as damage,
            // and TF counts for _double_ damage.
            return
                State.Brain +
                State.Heart +
                State.Sub +
                (2 * State.Damage);
        }

        public bool CanAffordCard(int buyPileIndex)
        {
            var card = State.BuyPile[buyPileIndex];

            if (State.Brain + State.Heart + State.Sub < card.RainbowCost)
                return false;

            if (State.Brain < card.BrainCost)
                return false;

            if (State.Heart < card.HeartCost)
                return false;

            if (State.Sub < card.SubCost)
                return false;

            return true;
        }

        private async Task EndTurnNormal()
        {
            await DiscardHand();

            // Allow the player to attack
            State.EnemyTF += State.Damage;
            await AnimationPlayer.DamageEnemy(State.Damage);

            // Move to the boss round if the enemy is defeated
            if (State.EnemyTF >= State.EnemyMaxTF)
            {
                await TriggerEffects(e => e.OnTurnEnd(this));
                await StartBossRound();
                return;
            }

            // Allow the enemy to attack.
            int enemyTfDamage = RollEnemyDamage() - State.Shield;
            enemyTfDamage = Math.Clamp(enemyTfDamage, 0, int.MaxValue);

            await TriggerEffects(e => e.OnPlayerAboutToTakeDamage(this, ref enemyTfDamage));
            State.PlayerTF += enemyTfDamage;
            await AnimationPlayer.DamagePlayer(enemyTfDamage);

            if (State.PlayerTF >= State.PlayerMaxTF)
            {
                EndBattle();
                return;
            }

            // Start the next turn
            await TriggerEffects(e => e.OnTurnEnd(this));

            State.TurnsElapsed++;
            await StartTurn();
        }

        private async Task EndTurnBoss()
        {
            await DiscardHand();

            // Allow the player to attack.

            int totalDamage = TotalDamageToBoss();
            State.EnemyTF += totalDamage;
            await AnimationPlayer.DamageEnemy(totalDamage);

            // End the battle if the boss is defeated
            if (State.EnemyTF >= State.EnemyMaxTF)
            {
                EndBattle();
                return;
            }

            // Allow the enemy to attack.
            // TODO: Use boss mechanics instead
            int enemyTfDamage = RollEnemyDamage() - State.Shield;
            enemyTfDamage = Math.Clamp(enemyTfDamage, 0, int.MaxValue);

            await TriggerEffects(e => e.OnPlayerAboutToTakeDamage(this, ref enemyTfDamage));
            State.PlayerTF += enemyTfDamage;
            await AnimationPlayer.DamagePlayer(enemyTfDamage);

            if (State.PlayerTF >= State.PlayerMaxTF)
            {
                EndBattle();
                return;
            }

            // Start the next turn
            await TriggerEffects(e => e.OnTurnEnd(this));

            State.TurnsElapsed++;
            await StartTurn();
        }

        private bool IsPermanentBuyPileCard(int buyPileIndex)
        {
            return buyPileIndex == State.BuyPile.Count - 1;
        }

        private void TransferAllCards(List<Card> src, List<Card> dst)
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
            return Rng.Next(EnemyMinTFDamage, EnemyMaxTFDamage + 1);
        }

        private void AssertBattleRunning()
        {
            if (BattleEnded)
                throw new Exception("You can't do that after the battle has ended");
        }

        private IEnumerable<Card> CardsOfferableAtTf(IEnumerable<Card> cards, int tf)
        {
            return cards
                .Where(c => tf <= c.MaxTF)
                .Where(c => tf >= c.MinTF);
        }

        private async Task StartBossRound()
        {
            State.IsBossRound = true;
            State.EnemyTF = 0;

            await AnimationPlayer.BossRoundStart();
            await StartTurn();
        }

        private async Task DebugCheatCardsIntoHand()
        {
            var cardsToCheat = State.CardRegistry
                .Cards
                .Values
                .Where(c => c.DebugCheatIntoHand);

            foreach (var card in cardsToCheat)
            {
                State.Hand.Add(card);
                await AnimationPlayer.DrawCard(card);
            }
        }
    }
}