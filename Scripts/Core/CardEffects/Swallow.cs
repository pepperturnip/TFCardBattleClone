using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class Swallow : ICardEffect
    {
        public async Task Activate(BattleController battle)
        {
            await battle.ResetBuyPile();

            for (int i = 0; i < 3; i++)
                await battle.DrawCard();
        }

        public string GetDescription(BattleState state)
            => "Draw 3, and reset the buy piles";

        public string GetOverriddenImage(BattleState state) => null;
    }
}