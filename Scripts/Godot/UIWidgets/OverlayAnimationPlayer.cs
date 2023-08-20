using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class OverlayAnimationPlayer : Control
    {
        public void Reset()
        {
            // HACK: Just play RESET on all AnimationPlayer nodes.
            // It might be better to just have _one_ AnimationPlayer that
            // contains all the animations, though, instead of trying to
            // keep track of all of them.
            foreach (var descendant in EnumerateDescendants(this))
            {
                if (descendant is AnimationPlayer animator)
                    animator.Play("RESET");
            }

            IEnumerable<Node> EnumerateChildren(Node parent)
            {
                for (int i = 0; i < parent.GetChildCount(); i++)
                {
                    yield return parent.GetChild<Node>(i);
                }
            }

            IEnumerable<Node> EnumerateDescendants(Node parent)
            {
                foreach (var child in EnumerateChildren(parent))
                {
                    foreach (var descendant in EnumerateDescendants(child))
                    {
                        yield return descendant;
                    }

                    yield return child;
                }
            }
        }

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

        public async Task PlayerWin()
        {
            var animator = GetNode<AnimationPlayer>("%BattleEndAnimator");
            animator.ResetAndPlay("PlayerWin");
            await ToSignal(animator, AnimationPlayer.SignalName.AnimationFinished);
        }

        public async Task PlayerLose()
        {
            var animator = GetNode<AnimationPlayer>("%BattleEndAnimator");
            animator.ResetAndPlay("PlayerLose");
            await ToSignal(animator, AnimationPlayer.SignalName.AnimationFinished);
        }
    }
}