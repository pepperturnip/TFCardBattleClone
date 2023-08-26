using System.Threading.Tasks;
namespace TFCardBattle.Core.LingeringEffects
{
    public class Suggestible : ILingeringEffect
    {
        public Task OnEnemyAboutToTakeDamage(
            BattleController battle,
            ref int damage
        )
        {
            damage += battle.State.Damage;
            return Task.CompletedTask;
        }
    }
}