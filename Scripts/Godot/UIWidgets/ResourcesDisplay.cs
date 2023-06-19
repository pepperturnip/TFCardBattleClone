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
        /// Forcibly sets the resource counters, skipping any kind of animation
        /// </summary>
        /// <param name="state"></param>
        public void RefreshResources(BattleState state)
        {
            _brain.RefreshValue(state.Brain);
            _heart.RefreshValue(state.Heart);
            _sub.RefreshValue(state.Sub);
            _shield.RefreshValue(state.Shield);
            _damage.RefreshValue(state.Damage);
        }
    }
}