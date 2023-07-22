using System;
using System.Collections.Generic;
using System.Linq;

namespace TFCardBattle.Core
{
    /// <summary>
    /// This class contains the state of a battle.
    /// It does NOT contain any game rules or logic.
    /// </summary>
    public class BattleState
    {
        public readonly ContentRegistry CardRegistry;
        public List<ILingeringEffect> LingeringEffects = new List<ILingeringEffect>();

        public PlayerLoadout PlayerLoadout;

        public int PlayerMaxTF = 100;
        public int PlayerTF = 0;

        public int EnemyTF = 0;
        public int EnemyMaxTF = 100;

        /// <summary>
        /// The cards currently on offer to the player this turn.
        /// Gets repopulated with random cards from
        /// <see cref="BattleState.OfferableCards"/> at the start of each turn.
        /// </summary>
        public List<Card> BuyPile = new List<Card>();

        /// <summary>
        /// All cards that the player owns, whether they're in the deck, hand,
        /// discard pile, or in-play.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Card> OwnedCards => Deck
                .Concat(Hand)
                .Concat(Discard)
                .Concat(InPlay);

        public List<Card> Deck = new List<Card>();
        public List<Card> Hand = new List<Card>();
        public List<Card> InPlay = new List<Card>();
        public List<Card> Discard = new List<Card>();

        public int Brain = 0;
        public int Heart = 0;
        public int Sub = 0;
        public int Shield = 0;
        public int Damage = 0;

        public DefaultDictionary<CustomResourceId, double> CustomResources
            = new DefaultDictionary<CustomResourceId, double>(0);

        public List<Consumable> Consumables = new List<Consumable>();

        /// <summary>
        /// The number of extra cards drawn this turn.
        /// Used for cards that have different effects depending on how much
        /// you've drawn, as well as for enemy traits that penalize you for
        /// drawing too much.
        /// </summary>
        public int DrawCount = 0;
        public int TurnsElapsed = 0;

        public BattleState(
            PlayerLoadout loadout,
            ContentRegistry registry
        )
        {
            PlayerLoadout = loadout;
            CardRegistry = registry;

            Deck.AddRange(loadout.StartingDeck);
        }

        public int GetResource(ResourceType resource)
        {
            switch (resource)
            {
                case ResourceType.Brain: return Brain;
                case ResourceType.Heart: return Heart;
                case ResourceType.Sub: return Sub;
                case ResourceType.Damage: return Damage;
                case ResourceType.Shield: return Shield;
                default: throw new NotImplementedException();
            }
        }

        public void SetResource(ResourceType resource, int value)
        {
            switch (resource)
            {
                case ResourceType.Brain: Brain = value; break;
                case ResourceType.Heart: Heart = value; break;
                case ResourceType.Sub: Sub = value; break;
                case ResourceType.Damage: Damage = value; break;
                case ResourceType.Shield: Shield = value; break;
                default: throw new NotImplementedException();
            }
        }

        public bool CanAffordCard(int buyPileIndex)
        {
            var card = BuyPile[buyPileIndex];

            if (Brain + Heart + Sub < card.RainbowCost)
                return false;

            if (Brain < card.BrainCost)
                return false;

            if (Heart < card.HeartCost)
                return false;

            if (Sub < card.SubCost)
                return false;

            return true;
        }
    }
}