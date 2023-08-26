using System.Threading.Tasks;

namespace TFCardBattle.Core.LingeringEffects
{
    public class BlondeHair : ILingeringEffect
    {
        public Task OnCardDrawn(BattleController battle, Card card)
        {
            // This should only trigger once per turn, so check that the draw
            // count is EXACTLY equal to 3 instead of greater than 3.
            if (battle.State.DrawCount == 3)
                battle.State.Damage += 3;

            return Task.CompletedTask;
        }
    }
}