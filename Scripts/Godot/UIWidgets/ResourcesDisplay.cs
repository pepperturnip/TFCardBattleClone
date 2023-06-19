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
            _brain.Value = state.Brain;
            _heart.Value = state.Heart;
            _sub.Value = state.Sub;
            _shield.Value = state.Shield;
            _damage.Value = state.Damage;
        }
    }
}