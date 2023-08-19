using System;
using System.Linq;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class ThemePackSelectionPage : Control
    {
        [Signal] public delegate void BackPressedEventHandler();
        [Signal] public delegate void ConfirmedEventHandler();

        [Export] public bool ShowBackButton = false;

        private Button _confirmButton => GetNode<Button>("%ConfirmButton");
        private Button _nextButton => GetNode<Button>("%NextButton");
        private Control _nextAndBackButtons => GetNode<Control>("%NextAndBackButtons");

        private readonly CardPack[] _brainChoices = ContentRegistry.CardPacks
            .Values
            .Where(p => p.Type == CardPackType.BrainSlot)
            .ToArray();

        private readonly CardPack[] _heartChoices = ContentRegistry.CardPacks
            .Values
            .Where(p => p.Type == CardPackType.HeartSlot)
            .ToArray();

        private readonly CardPack[] _subChoices = ContentRegistry.CardPacks
            .Values
            .Where(p => p.Type == CardPackType.SubSlot)
            .ToArray();

        private PlayerLoadout _loadout;

        private ThemePackPicker _picker => GetNode<ThemePackPicker>("%ThemePackPicker");

        public override void _Ready()
        {
            _picker.SetChoices(_brainChoices, _heartChoices, _subChoices);
        }

        public override void _Process(double delta)
        {
            _confirmButton.Visible = !ShowBackButton;
            _nextAndBackButtons.Visible = ShowBackButton;
        }

        public void Init(PlayerLoadout loadout)
        {
            _loadout = loadout;
            _picker.SetSelectedPacks(_loadout.ThemePacks);
        }

        public void OnSelectionChanged()
        {
            if (_loadout == null)
                return;

            _confirmButton.Disabled = !_picker.SelectionsValid;
            _nextButton.Disabled = !_picker.SelectionsValid;

            if (!_picker.SelectionsValid)
                return;

            _loadout.ThemePacks = _picker.SelectedPacks.ToArray();
        }

        public void OnConfirmButtonPressed()
        {
            EmitSignal(SignalName.Confirmed);
        }

        public void OnBackButtonPressed()
        {
            EmitSignal(SignalName.BackPressed);
        }
    }
}