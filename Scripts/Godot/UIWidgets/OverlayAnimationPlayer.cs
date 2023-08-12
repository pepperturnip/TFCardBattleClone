using System;
using System.Threading.Tasks;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class OverlayAnimationPlayer : Control
    {
        public Task DamagePlayer(int damageAmount)
            => DamageAnimation("DamagePlayer", damageAmount);

        public Task DamageEnemy(int damageAmount)
            => DamageAnimation("DamageEnemy", damageAmount);

        private async Task DamageAnimation(string animName, int damageAmount)
        {
            var animator = GetNode<AnimationPlayer>("%DamageAnimator");
            var label = GetNode<Label>("%DamageAnimationLabel");

            if (damageAmount <= 0)
                return;

            label.Text = $"+{damageAmount}";
            animator.ResetAndPlay(animName);
            await ToSignal(animator, AnimationPlayer.SignalName.AnimationFinished);
        }

        public async Task BossRoundStart()
        {
            var animator = GetNode<AnimationPlayer>("%BossStartAnimator");
            animator.ResetAndPlay("Start");
            await ToSignal(animator, AnimationPlayer.SignalName.AnimationFinished);
        }
    }
}