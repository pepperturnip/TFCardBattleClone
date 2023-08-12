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

        public async Task BossRoundStart()
        {
            var animator = GetNode<AnimationPlayer>("%BossStartAnimator");
            animator.ResetAndPlay("Start");
            await ToSignal(animator, AnimationPlayer.SignalName.AnimationFinished);
        }
    }
}