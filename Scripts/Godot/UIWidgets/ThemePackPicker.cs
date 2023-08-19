using System;
using System.Collections.Generic;
using System.Linq;
using TFCardBattle.Core;
using Godot;
using Newtonsoft.Json;

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
            InitPicker(_brainPacks, brainChoices, RequiredBrainCount);
            InitPicker(_heartPacks, heartChoices, RequiredHeartCount);
            InitPicker(_subPacks, subChoices, RequiredSubCount);
        }

        public void OnChildSelectionChanged()
        {
            EmitSignal(SignalName.SelectionChanged);
        }

        private void InitPicker(
            SingleSuitThemePackPicker picker,
            CardPack[] choices,
            int requiredCount
        )
        {
            string defaultSelectionsJson = FileAccess.GetFileAsString("res://Content/DefaultLoadout.json");
            var defaultSelections = JsonConvert.DeserializeObject<CardPackId[]>(defaultSelectionsJson)
                .Select(id => ContentRegistry.CardPacks[id])
                .ToArray();

            picker.SetChoices(choices, defaultSelections, requiredCount);
        }
    }
}
