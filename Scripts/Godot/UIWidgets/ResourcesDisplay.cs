using System;
using System.Collections.Generic;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class ResourcesDisplay : Control
    {
        private readonly ResourceType[] _resources = Enum.GetValues<ResourceType>();

        private Control _template;

        public override void _Ready()
        {
            _template = GetNode<Control>("%LabelTemplate");
            RemoveChild(_template);
        }

        /// <summary>
        /// Updates the resource counters, but using a smooth "counting"
        /// animation.
        /// </summary>
        /// <param name="state"></param>
        public void UpdateResources(BattleState state)
        {
            foreach (var resource in _resources)
            {
                var label = GetValueLabel(resource);
                label.AccumulateToValue(state.GetResource(resource));
            }
        }

        /// <summary>
        /// Forcibly sets the counters to 0, skipping any kind of animation
        /// </summary>
        /// <param name="state"></param>
        public void DiscardResources()
        {
            foreach (var resource in _resources)
            {
                var label = GetValueLabel(resource);
                label.RefreshValue(0);
            }
        }

        public override void _Process(double delta)
        {
            foreach (var resource in _resources)
            {
                var resourceDisplay = GetResourceDisplay(resource);
                var valueLabel = GetValueLabel(resource);
                resourceDisplay.Visible = valueLabel.DisplayedValue != 0 || valueLabel.AccumulatedDelta != 0;
            }
        }

        private Control GetResourceDisplay(ResourceType resource)
        {
            var control = GetNodeOrNull<Control>(resource.ToString());
            if (control != null)
                return control;

            // It doesn't already exist, so create it from the template
            var resourceDisplay = (Control)_template.Duplicate();
            resourceDisplay.Name = resource.ToString();
            resourceDisplay.GetNode<Label>("Label").Text = resource.ToString();
            AddChild(resourceDisplay);

            return resourceDisplay;
        }

        private AccumulatingLabel GetValueLabel(ResourceType resource)
            => GetResourceDisplay(resource).GetNode<AccumulatingLabel>("ValueLabel");
    }
}