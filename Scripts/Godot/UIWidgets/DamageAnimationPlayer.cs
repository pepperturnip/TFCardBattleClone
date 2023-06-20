using System.Threading.Tasks;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class DamageAnimationPlayer : Control
    {
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%Animator");
        private Label _label => GetNode<Label>("%DamageAnimationLabel");

        public Task DamagePlayer(int damageAmount)
            => DamageAnimation("DamagePlayer", damageAmount);

        public Task DamageEnemy(int damageAmount)
            => DamageAnimation("DamageEnemy", damageAmount);

        private async Task DamageAnimation(string animName, int damageAmount)
        {
            if (damageAmount <= 0)
                return;

            _label.Text = $"+{damageAmount}";
            _animator.ResetAndPlay(animName);
            await ToSignal(_animator, AnimationPlayer.SignalName.AnimationFinished);
        }
    }
}