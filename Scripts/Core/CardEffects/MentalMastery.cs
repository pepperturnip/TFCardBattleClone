using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class MentalMastery : ICardEffect
    {
        public async Task Activate(BattleController battle)
        {
            await battle.ForgetBasicCard();
            await battle.DrawCard();
        }

        public string GetDescription(BattleState state)
            => "Remove a basic card from your deck.  Draw 1.";

        public string GetOverriddenImage(BattleState state) => null;
    }
}