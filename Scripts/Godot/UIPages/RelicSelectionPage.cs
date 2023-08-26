using System;
using System.Linq;
using System.Collections.Generic;

using TFCardBattle.Core;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class RelicSelectionPage : Control
    {
        [Signal] public delegate void ConfirmedEventHandler();

        [Export] public PackedScene RelicButtonPrefab;

        private PlayerLoadout _loadout;

        private Control _choicesContainer => GetNode<Control>("%ChoicesContainer");

        private readonly Random _rng = new Random((int)GD.Randi());

        public void Init(PlayerLoadout loadout)
        {
            _loadout = loadout;

            // Clear out the old choices and choose new ones
            while (_choicesContainer.GetChildCount() > 0)
            {
                var child = _choicesContainer.GetChild(0);

                _choicesContainer.RemoveChild(child);
                child.QueueFree();
            }

            // Randomly select 3 relics to offer the player
            var offerableRelics = ContentRegistry.Relics
                .Values
                .Where(r => !_loadout.Relics.Contains(r))
                .ToList();

            var choices = new List<Relic>();
            while (offerableRelics.Count > 0 && choices.Count < 3)
            {
                var relic = _rng.PickFrom(offerableRelics);
                offerableRelics.Remove(relic);
                choices.Add(relic);
            }

            // Create buttons for the offered relics
            for (int i = 0; i < choices.Count; i++)
            {
                var relic = choices[i];

                var button = RelicButtonPrefab.Instantiate<RelicCardButton>();
                button.Pressed += () => OnRelicChosen(relic);
                button.SetRelic(relic);

                if (ResourceLoader.Exists(relic.IconPath))
                {
                    button.Icon = ResourceLoader.Load<Texture2D>(relic.IconPath);
                    button.ExpandIcon = true;
                }

                _choicesContainer.AddChild(button);
            }
        }

        public void OnChooseNoneButtonPressed()
        {
            EmitSignal(SignalName.Confirmed);
        }

        private void OnRelicChosen(Relic relic)
        {
            _loadout.Relics.Add(relic);
            EmitSignal(SignalName.Confirmed);
        }
    }
}