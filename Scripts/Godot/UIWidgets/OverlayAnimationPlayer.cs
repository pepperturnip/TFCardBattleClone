using System;
using System.Threading.Tasks;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class OverlayAnimationPlayer : Control
    {
        private DamageAnimationPlayer _damageAnimationPlayer => GetNode<DamageAnimationPlayer>("%DamageAnimationPlayer");

        public Task DamagePlayer(int damageAmount)
            => _damageAnimationPlayer.DamagePlayer(damageAmount);

        public Task DamageEnemy(int damageAmount)
            => _damageAnimationPlayer.DamageEnemy(damageAmount);

        public Task BossRoundStart()
        {
            GetNode("%BossStartAnimationPlayer").Call("Play");
            return WaitFor.Seconds(1.8);
        }
    }
}