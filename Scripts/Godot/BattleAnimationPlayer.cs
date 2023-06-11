using System;
using System.Threading.Tasks;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class BattleAnimationPlayer : Node, IBattleAnimationPlayer
    {
        [Signal] public delegate void IsAnimatingChangedEventHandler(bool isAnimating);

        public bool IsAnimating
        {
            get => _isAnimating;
            set
            {
                _isAnimating = value;
                EmitSignal(SignalName.IsAnimatingChanged, value);
            }
        }
        private bool _isAnimating;

        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%Animator");
        private CardRowDisplay _handDisplay => GetNode<CardRowDisplay>("%HandDisplay");

        public async Task DamageEnemy(int damageAmount)
        {
            using (SetAnimating())
            {
                await DamageAnimation("DamageEnemy", damageAmount);

                var tfBar = GetNode<TFBar>("%EnemyTFBar");
                var tween = GetTree().CreateTween();
                tween.TweenProperty(tfBar, "value", tfBar.Value + damageAmount, 1);
            }
        }

        public async Task DamagePlayer(int damageAmount)
        {
            using (SetAnimating())
            {
                await DamageAnimation("DamagePlayer", damageAmount);

                var tfBar = GetNode<TFBar>("%PlayerTFBar");
                var tween = GetTree().CreateTween();
                tween.TweenProperty(tfBar, "value", tfBar.Value + damageAmount, 1);
            }
        }

        public async Task DrawCard(ICard card)
        {
            using (SetAnimating())
            {
                _handDisplay.AddCard(card);
                await WaitFor.Seconds(0.125);
            }
        }

        public async Task PlayCard(int handIndexPlayed, BattleState newState)
        {
            using (SetAnimating())
            {
                _handDisplay.RemoveCard(handIndexPlayed);
                await WaitFor.Seconds(0.125);
            }
        }

        public async Task DiscardHand()
        {
            using (SetAnimating())
            {
                _handDisplay.Refresh(Array.Empty<ICard>());

                // Silence that dumb "you didn't await anything" warning
                await Task.Yield();
            }
        }

        private async Task DamageAnimation(string animationName, int damageAmount)
        {
            if (damageAmount <= 0)
                return;

            GetNode<Label>("%DamageAnimationLabel").Text = $"+{damageAmount}";
            _animator.ResetAndPlay(animationName);
            await ToSignal(_animator, AnimationPlayer.SignalName.AnimationFinished);
        }

        private IDisposable SetAnimating() => new IsAnimatingDisposable(this);

        private class IsAnimatingDisposable : IDisposable
        {
            private BattleAnimationPlayer _owner;

            public IsAnimatingDisposable(BattleAnimationPlayer owner)
            {
                _owner = owner;

                if (_owner.IsAnimating)
                    throw new InvalidOperationException("It's already animating!");

                _owner.IsAnimating = true;
            }

            public void Dispose()
            {
                _owner.IsAnimating = false;
            }
        }
    }
}