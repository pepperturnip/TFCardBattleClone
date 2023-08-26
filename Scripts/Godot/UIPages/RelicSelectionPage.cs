using System;
using System.Linq;

using TFCardBattle.Core;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class RelicSelectionPage : Control
    {
        [Signal] public delegate void ConfirmedEventHandler();

        private PlayerLoadout _loadout;

        private Relic[] _choices;

        private Control _choicesContainer => GetNode<Control>("%ChoicesContainer");

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

            // TODO: Randomly select 3 relics to offer the player, instead of
            // offering all of them
            _choices = ContentRegistry.Relics
                .Values
                .Where(r => !_loadout.Relics.Contains(r))
                .OrderBy(r => r.Name)
                .ToArray();

            for (int i = 0; i < _choices.Length; i++)
            {
                var relic = _choices[i];

                var button = new Button();
                button.Text = relic.Name;
                button.TooltipText = $"+{relic.TFCost} TF.  {relic.Description}";
                button.Pressed += () => OnRelicChosen(relic);

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