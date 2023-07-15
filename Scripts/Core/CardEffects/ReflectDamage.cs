using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class ReflectDamage : ICardEffect, ILingeringEffect
    {
        public Task Activate(BattleController battle)
        {
            battle.AddEffect(this);
            return Task.CompletedTask;
        }

        public string GetDescription(BattleState state)
            => "All TF that would be dealt to you this turn is dealt to the opponent instead.";

        public string GetOverriddenImage(BattleState state) => null;

        Task ILingeringEffect.OnPlayerAboutToTakeDamage(BattleController battle, ref int damage)
        {
            battle.RemoveEffect(this);

            int dmg = damage;
            damage = 0;

            battle.State.EnemyTF += dmg;
            return battle.AnimationPlayer.DamageEnemy(dmg);
        }
    }
}