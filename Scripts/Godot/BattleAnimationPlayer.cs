using System;
using System.Threading.Tasks;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class BattleAnimationPlayer : Node, IBattleAnimationPlayer
    {
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%Animator");
        private CardRowDisplay _handDisplay => GetNode<CardRowDisplay>("%HandDisplay");

        public async Task DamageEnemy(int damageAmount)
        {
            await DamageAnimation("DamageEnemy", damageAmount);

            await FillTFBar(
                GetNode<ProgressBar>("%EnemyTFBar"),
                GetNode<Label>("%EnemyTFLabel"),
                damageAmount,
                0.5
            );
        }
        public async Task DamagePlayer(int damageAmount)
        {
            await DamageAnimation("DamagePlayer", damageAmount);

            await FillTFBar(
                GetNode<ProgressBar>("%PlayerTFBar"),
                GetNode<Label>("%PlayerTFLabel"),
                damageAmount,
                0.5
            );
        }

        public Task DrawCard(ICard card)
        {
            _handDisplay.AddCard(card);
            return WaitFor.Seconds(0.125);
        }

        public Task PlayCard(int handIndexPlayed, BattleState newState)
        {
            _handDisplay.RemoveCard(handIndexPlayed);
            return WaitFor.Seconds(0.125);
        }

        public Task DiscardHand()
        {
            _handDisplay.Refresh(Array.Empty<ICard>());
            return Task.CompletedTask;
        }

        private async Task DamageAnimation(string animationName, int damageAmount)
        {
            if (damageAmount <= 0)
                return;

            GetNode<Label>("%DamageAnimationLabel").Text = $"+{damageAmount}";
            _animator.ResetAndPlay(animationName);
            await ToSignal(_animator, AnimationPlayer.SignalName.AnimationFinished);
        }

        private async Task FillTFBar(ProgressBar bar, Label label, int damageAmount, double duration)
        {
            if (damageAmount <= 0)
                return;

            double startValue = bar.Value;
            double targetValue = bar.Value + damageAmount;
            double timer = 0;

            while (timer < duration)
            {
                double delta = await WaitFor.NextFrame();
                timer += delta;

                bar.Value = Mathf.Lerp(startValue, targetValue, timer / duration);
                label.Text = $"{(int)bar.Value} / {(int)bar.MaxValue}";
            }

            bar.Value = targetValue;
            label.Text = $"{targetValue} / {(int)bar.MaxValue}";
        }
    }
}