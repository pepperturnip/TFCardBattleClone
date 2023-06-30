using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class JustTheTip : ICardEffect
    {
        public Task Activate(BattleController battle)
        {
            battle.State.Shield -= 3;
            battle.State.Damage *= 2;
            return Task.CompletedTask;
        }

        public string GetDescription(BattleState state)
            => "Shield: -3\nTF: x2";

        public string GetOverriddenImage(BattleState state) => null;
    }
}