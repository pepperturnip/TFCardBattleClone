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
            Card copy = battle.State.InPlay.Last();

            // Add brain equal to the number of copies the player owns.
            //
            // This is technically different from what the original game does.
            // The original game increments a counter every time you play this
            // card, and that counter is what gets added to your brain.
            //
            // I'd rather not add a separate field to BattleState just for this
            // one card, though, so let's just count the copies owned instead.
            // And, honestly, this way is just more intuitive.
            battle.State.Brain += battle.State.OwnedCards.Count(c => c == copy);

            // Add a copy of this card to the discard pile
            battle.State.Discard.Add(copy);
            return Task.CompletedTask;
        }

        public string GetDescription(BattleState state)
            => "Add brain equal to the number of copies of this card in your deck.  Add a copy of this card to your discard pile.";

        public string GetOverriddenImage(BattleState state) => null;
    }
}