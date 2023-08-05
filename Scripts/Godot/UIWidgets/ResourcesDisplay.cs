using System;
using System.Collections.Generic;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class ResourcesDisplay : Control
    {
        [Export] public PackedScene ResourceCounterPrefab;

        private readonly ResourceType[] _resources = Enum.GetValues<ResourceType>();
        private readonly HashSet<CustomResourceId> _customResources = new HashSet<CustomResourceId>();

        /// <summary>
        /// Updates the resource counters, but using a smooth "counting"
        /// animation.
        /// </summary>
        /// <param name="state"></param>
        public void UpdateResources(BattleState state)
        {
            foreach (var resource in _resources)
            {
                var counter = GetResourceCounter(resource);
                counter.AccumulateToValue(state.GetResource(resource));
            }

            foreach (var id in state.CustomResources.Keys)
            {
                _customResources.Add(id);
                var counter = GetResourceCounter(id);
                counter.AccumulateToValue(state.CustomResources[id]);
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
                var counter = GetResourceCounter(resource);
                counter.RefreshValue(0);
            }

            // Note: we're intentionally NOT discarding the custom resources,
            // because they don't reset at the end of every turn.
        }

        private ResourceCounter GetResourceCounter(ResourceType resource)
            => GetNode<ResourceCounter>(resource.ToString());

        private ResourceCounter GetResourceCounter(CustomResourceId id)
        {
            var control = GetNodeOrNull<ResourceCounter>(id.ToString());
            if (control != null)
                return control;

            // It doesn't already exist, so create it from the template
            CustomResource resource = ContentRegistry.CustomResources[id];

            var counter = ResourceCounterPrefab.Instantiate<ResourceCounter>();
            counter.Name = id.ToString();
            counter.Texture = ResourceLoader.Load<Texture2D>(resource.IconPath);
            AddChild(counter);

            return counter;
        }
    }
}