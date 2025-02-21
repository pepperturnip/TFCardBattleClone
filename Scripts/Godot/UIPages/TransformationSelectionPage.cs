using System;
using System.Linq;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class TransformationSelectionPage : Control
    {
        [Signal] public delegate void ConfirmedEventHandler();

        private readonly Transformation[] _choices = ContentRegistry
            .Transformations
            .Values
            .ToArray();

        private PlayerLoadout _loadout;

        private TransformationPicker _picker => GetNode<TransformationPicker>("%TransformationPicker");

        public override void _Ready()
        {
            _picker.SetChoices(_choices);
        }

        public void Init(PlayerLoadout loadout)
        {
            _loadout = loadout;

            int selectionIndex = _loadout.Transformation == null
                ? 0
                : Array.IndexOf(_choices, _loadout.Transformation);

            _picker.Select(selectionIndex);
            OnSelectionChanged(selectionIndex);

            // HACK: Add all the debug relics to the loadout
            // TODO: Move this to the relic selection page
            foreach (var relic in ContentRegistry.Relics.Values)
            {
                if (relic.DebugAlwaysAdd)
                    loadout.Relics.Add(relic);
            }
        }

        public void OnSelectionChanged(int index)
        {
            if (_loadout == null)
                return;

            _loadout.Transformation = _choices[index];
            GD.Print("Selection changed");
        }

        public void OnConfirmButtonPressed()
        {
            EmitSignal(SignalName.Confirmed);
        }
    }
}