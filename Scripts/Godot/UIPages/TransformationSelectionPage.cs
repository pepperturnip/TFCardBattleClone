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
        }

        public void OnSelectionChanged(int index)
        {
            if (_loadout == null)
                return;

            _loadout.Transformation = _choices[index];
        }

        public void OnConfirmButtonPressed()
        {
            EmitSignal(SignalName.Confirmed);
        }
    }
}