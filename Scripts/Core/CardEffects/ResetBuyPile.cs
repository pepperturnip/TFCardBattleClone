using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class ResetBuyPile : ICardEffect
    {
        public Task Activate(BattleController battle)
        {
            return battle.ResetBuyPile();
        }

        public string GetDescription(BattleState state) => "Reset the buy piles";
        public string GetOverriddenImage(BattleState state) => null;
    }
}