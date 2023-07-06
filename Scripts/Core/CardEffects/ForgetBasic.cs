using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class ForgetBasic : ICardEffect
    {
        public async Task Activate(BattleController battle)
        {
            await battle.ForgetBasicCard();
        }

        public string GetDescription(BattleState state)
            => "Remove a basic card from your deck.";

        public string GetOverriddenImage(BattleState state) => null;
    }
}