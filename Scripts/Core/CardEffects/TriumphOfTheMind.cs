using System;
using System.Linq;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class TriumphOfTheMind : ICardEffect
    {
        public async Task Activate(BattleController battle)
        {
            bool anyHeart = battle.State
                .OwnedCards
                .Any(c => c.HeartCost > 0);

            if (!anyHeart)
                await battle.TakeExtraTurn();
        }

        public string GetDescription(BattleState state)
            => "Take an extra turn, if you own no heart cards.";

        // TODO: Change the description and texture if the player has heart
        // cards in their deck.
        public string GetOverriddenImage(BattleState state) => null;
    }
}