using System;
using System.Threading.Tasks;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class BattleAnimationPlayer : Node, IBattleAnimationPlayer
    {
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%Animator");

        public async Task DamageEnemy(int damageAmount)
        {
            if (damageAmount <= 0)
                return;

            GetNode<Label>("%DamageAnimationLabel").Text = $"+{damageAmount}";
            _animator.ResetAndPlay("Damage");
            await ToSignal(_animator, AnimationPlayer.SignalName.AnimationFinished);
        }

        public Task DamagePlayer(int damageAmount) => DamageEnemy(damageAmount);
    }
}