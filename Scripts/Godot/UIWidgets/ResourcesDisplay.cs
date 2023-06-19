using System;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class ResourcesDisplay : Control
    {
        private AccumulatingLabel _brain => GetNode<AccumulatingLabel>("%BrainLabel");
        private AccumulatingLabel _heart => GetNode<AccumulatingLabel>("%HeartLabel");
        private AccumulatingLabel _sub => GetNode<AccumulatingLabel>("%SubLabel");
        private AccumulatingLabel _shield => GetNode<AccumulatingLabel>("%ShieldLabel");
        private AccumulatingLabel _damage => GetNode<AccumulatingLabel>("%DamageResourceLabel");

        /// <summary>
        /// Updates the resource counters, but using a smooth "counting"
        /// animation.
        /// </summary>
        /// <param name="state"></param>
        public void UpdateResources(BattleState state)
        {
            _brain.AccumulateToValue(state.Brain);
            _heart.AccumulateToValue(state.Heart);
            _sub.AccumulateToValue(state.Sub);
            _shield.AccumulateToValue(state.Shield);
            _damage.AccumulateToValue(state.Damage);
        }

        /// <summary>
        /// Forcibly sets the counters to 0, skipping any kind of animation
        /// </summary>
        /// <param name="state"></param>
        public void DiscardResources()
        {
            _brain.RefreshValue(0);
            _heart.RefreshValue(0);
            _sub.RefreshValue(0);
            _shield.RefreshValue(0);
            _damage.RefreshValue(0);
        }
    }
}