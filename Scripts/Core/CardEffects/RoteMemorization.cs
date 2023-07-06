using System;
using System.Threading.Tasks;
using System.Linq;

namespace TFCardBattle.Core.CardEffects
{
    public class RoteMemorization : ICardEffect
    {
        public Task Activate(BattleController battle)
        {
            // HACK: Get a copy of this card by looking at the in-play cards
            // This only works because we know BattleController puts the card
            // in the in-play list before calling Activate().
            Card copy = battle.State.CardsPlayedThisTurn.Last();

            // Add a copy of this card to the discard pile
            battle.State.Discard.Add(copy);

            // Add brain equal to the number of copies in the deck
            battle.State.Brain += battle.State.Deck.Count(c => c == copy);

            return Task.CompletedTask;
        }

        public string GetDescription(BattleState state)
            => "Add brain equal to the number of copies of this card in your deck.  Add a copy of this card to your discard pile.";

        public string GetOverriddenImage(BattleState state) => null;
    }
}