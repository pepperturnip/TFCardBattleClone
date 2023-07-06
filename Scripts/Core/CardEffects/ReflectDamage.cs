using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class ReflectDamage : ICardEffect
    {
        public Task Activate(BattleController battle)
        {
            battle.State.ReflectDamage = true;
            return Task.CompletedTask;
        }

        public string GetDescription(BattleState state)
            => "All TF that would be dealt to you this turn is dealt to the opponent instead.";

        public string GetOverriddenImage(BattleState state) => null;
    }
}