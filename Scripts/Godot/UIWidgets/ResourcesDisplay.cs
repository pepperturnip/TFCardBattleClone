using System;
using System.Collections.Generic;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class ResourcesDisplay : Control
    {
        private readonly ResourceType[] _resources = Enum.GetValues<ResourceType>();
        private readonly HashSet<CustomResourceId> _customResources = new HashSet<CustomResourceId>();

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

            foreach (var id in state.CustomResources.Keys)
            {
                _customResources.Add(id);
                var label = GetValueLabel(id);
                label.AccumulateToValue(state.CustomResources[id]);
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

            // Note: we're intentionally NOT discarding the custom resources,
            // because they don't reset at the end of every turn.
        }

        public override void _Process(double delta)
        {
            foreach (var resource in _resources)
            {
                var resourceDisplay = GetResourceDisplay(resource);
                var valueLabel = GetValueLabel(resource);
                resourceDisplay.Visible = valueLabel.DisplayedValue != 0 || valueLabel.AccumulatedDelta != 0;
            }

            foreach (var id in _customResources)
            {
                var resourceDisplay = GetResourceDisplay(id);
                var valueLabel = GetValueLabel(id);
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

        private Control GetResourceDisplay(CustomResourceId resource)
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

        private AccumulatingLabel GetValueLabel(CustomResourceId resource)
            => GetResourceDisplay(resource).GetNode<AccumulatingLabel>("ValueLabel");
    }
}