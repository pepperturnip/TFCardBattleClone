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

        private DamageAnimationPlayer _damageAnimator => GetNode<DamageAnimationPlayer>("%DamageAnimationPlayer");
        private HandDisplay _handDisplay => GetNode<HandDisplay>("%HandDisplay");
        private BuyPileDisplay _buyPileDisplay => GetNode<BuyPileDisplay>("%BuyPileDisplay");
        private ResourcesDisplay _resourcesDisplay => GetNode<ResourcesDisplay>("%ResourcesDisplay");

        public async Task DamageEnemy(int damageAmount)
        {
            using (SetAnimating())
            {
                await _damageAnimator.DamageEnemy(damageAmount);

                var tfBar = GetNode<TFBar>("%EnemyTFBar");
                var tween = GetTree().CreateTween();
                tween.SetTrans(Tween.TransitionType.Bounce);
                tween.TweenProperty(tfBar, "Value", tfBar.Value + damageAmount, 2);
            }
        }

        public async Task DamagePlayer(int damageAmount)
        {
            using (SetAnimating())
            {
                await _damageAnimator.DamagePlayer(damageAmount);

                // Don't wait for the bar to finish moving.
                // This ain't Pokemon Diamond.
                var tfBar = GetNode<TFBar>("%PlayerTFBar");
                var tween = GetTree().CreateTween();
                tween.SetTrans(Tween.TransitionType.Bounce);
                tween.TweenProperty(tfBar, "Value", tfBar.Value + damageAmount, 2);
            }
        }

        public async Task DrawCard(Card card)
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
                _handDisplay.PlayActivateAnimation(handIndexPlayed);
                _handDisplay.RemoveCard(handIndexPlayed);
                await WaitFor.Seconds(0.125);
            }
        }

        public Task DiscardResources()
        {
            _resourcesDisplay.DiscardResources();
            return Task.CompletedTask;
        }

        public async Task DiscardHand()
        {
            using (SetAnimating())
            {
                await TweenPos(_handDisplay, Vector2.Down * 208, 0.1);
                _handDisplay.ClearCards();
                _handDisplay.Position = Vector2.Zero;
            }
        }

        public async Task RefreshBuyPile(Card[] cards)
        {
            const double moveTime = 0.1;

            using (SetAnimating())
            {
                await TweenPos(_buyPileDisplay, Vector2.Up * 208, moveTime);
                _buyPileDisplay.Refresh();
                await TweenPos(_buyPileDisplay, Vector2.Zero, moveTime);
            }
        }

        public async Task BuyCard(int buyPileIndex, bool isPermanentBuyPileCard)
        {
            using (SetAnimating())
            {
                _buyPileDisplay.PlayBuyAnimation(buyPileIndex);

                if (!isPermanentBuyPileCard)
                    _buyPileDisplay.RemoveCard(buyPileIndex);

                await WaitFor.Seconds(0.125);
            }
        }

        public async Task ForgetCard(Card card, BattleState state)
        {
            using (SetAnimating())
            {
                GetNode<ForgetAnimationPlayer>("%ForgetAnimationPlayer").QueueForget(card, state);
                await WaitFor.Seconds(0.25);
            }
        }

        private async Task TweenPos(GodotObject node, Vector2 destination, double duration)
        {
            var tween = GetTree().CreateTween();

            tween.TweenProperty(
                node,
                "position",
                destination,
                duration
            );
            await tween.ToSignal(tween, Tween.SignalName.Finished);
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