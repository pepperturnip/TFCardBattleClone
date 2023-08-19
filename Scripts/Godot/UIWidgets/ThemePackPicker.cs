using System;
using System.Collections.Generic;
using System.Linq;
using TFCardBattle.Core;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class ThemePackPicker : Control
    {
        [Signal] public delegate void SelectionChangedEventHandler();

        public bool SelectionsValid =>
            _brainPacks.SelectionsValid &&
            _heartPacks.SelectionsValid &&
            _subPacks.SelectionsValid;

        public IEnumerable<CardPack> SelectedPacks => _brainPacks.SelectedPacks
            .Concat(_heartPacks.SelectedPacks)
            .Concat(_subPacks.SelectedPacks);

        private SingleSuitThemePackPicker _brainPacks => GetNode<SingleSuitThemePackPicker>("%BrainPacks");
        private SingleSuitThemePackPicker _heartPacks => GetNode<SingleSuitThemePackPicker>("%HeartPacks");
        private SingleSuitThemePackPicker _subPacks => GetNode<SingleSuitThemePackPicker>("%SubPacks");

        private const int RequiredBrainCount = 5;
        private const int RequiredHeartCount = 4;
        private const int RequiredSubCount = 4;

        public void SetChoices(
            CardPack[] brainChoices,
            CardPack[] heartChoices,
            CardPack[] subChoices
        )
        {
            _brainPacks.SetChoices(brainChoices, RequiredBrainCount);
            _heartPacks.SetChoices(heartChoices, RequiredHeartCount);
            _subPacks.SetChoices(subChoices, RequiredSubCount);
        }

        public void SetSelectedPacks(IEnumerable<CardPack> selectedPacks)
        {
            _brainPacks.SetSelectedPacks(selectedPacks.Where(p => p.Type == CardPackType.BrainSlot));
            _heartPacks.SetSelectedPacks(selectedPacks.Where(p => p.Type == CardPackType.HeartSlot));
            _subPacks.SetSelectedPacks(selectedPacks.Where(p => p.Type == CardPackType.SubSlot));
        }

        public void OnChildSelectionChanged()
        {
            EmitSignal(SignalName.SelectionChanged);
        }
    }
}
