using System;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class ResourcesDisplay : Control
    {
        /// <summary>
        /// Updates the resource counters, but using a smooth "counting"
        /// animation.
        /// </summary>
        /// <param name="state"></param>
        public void UpdateResources(BattleState state)
        {
            var resources = new[]
            {
                ResourceType.Brain,
                ResourceType.Heart,
                ResourceType.Sub,
                ResourceType.Shield,
                ResourceType.Damage
            };

            foreach (var resource in resources)
            {
                var label = GetAccumulatingLabel(resource);
                label.AccumulateToValue(state.GetResource(resource));
            }
        }

        /// <summary>
        /// Forcibly sets the counters to 0, skipping any kind of animation
        /// </summary>
        /// <param name="state"></param>
        public void DiscardResources()
        {
            var resources = new[]
            {
                ResourceType.Brain,
                ResourceType.Heart,
                ResourceType.Sub,
                ResourceType.Shield,
                ResourceType.Damage
            };

            foreach (var resource in resources)
            {
                var label = GetAccumulatingLabel(resource);
                label.RefreshValue(0);
            }
        }

        private AccumulatingLabel GetAccumulatingLabel(ResourceType resource)
            => GetNode<AccumulatingLabel>($"{resource}/ValueLabel");
    }
}