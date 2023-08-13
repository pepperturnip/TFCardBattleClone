using System;
using System.Collections.Generic;
using System.Linq;
using TFCardBattle.Core;
using Godot;
using Newtonsoft.Json;

namespace TFCardBattle.Godot
{
    public partial class PlayerLoadoutSelection : Control
    {
        private TransformationPicker _transformationPicker => GetNode<TransformationPicker>("%TransformationPicker");
        private SingleSuitThemePackPicker _brainPacks => GetNode<SingleSuitThemePackPicker>("%BrainPacks");
        private SingleSuitThemePackPicker _heartPacks => GetNode<SingleSuitThemePackPicker>("%HeartPacks");
        private SingleSuitThemePackPicker _subPacks => GetNode<SingleSuitThemePackPicker>("%SubPacks");

        private const int RequiredBrainCount = 5;
        private const int RequiredHeartCount = 4;
        private const int RequiredSubCount = 4;

        public override void _Ready()
        {
            InitTransformationPicker();
            InitThemePackPicker(_brainPacks, CardPackType.BrainSlot, RequiredBrainCount);
            InitThemePackPicker(_heartPacks, CardPackType.HeartSlot, RequiredHeartCount);
            InitThemePackPicker(_subPacks, CardPackType.SubSlot, RequiredSubCount);
        }

        private void InitTransformationPicker()
        {
            var choices = ContentRegistry.Transformations.Values.ToArray();
            _transformationPicker.SetChoices(choices);
        }

        private void InitThemePackPicker(
            SingleSuitThemePackPicker picker,
            CardPackType packType,
            int requiredCount
        )
        {
            var themePackChoices = ContentRegistry.CardPacks
                .Values
                .Where(p => p.Type == packType)
                .ToArray();

            string defaultSelectionsJson = FileAccess.GetFileAsString("res://Content/DefaultLoadout.json");
            var defaultSelections = JsonConvert.DeserializeObject<CardPackId[]>(defaultSelectionsJson)
                .Select(id => ContentRegistry.CardPacks[id])
                .ToArray();

            picker.SetChoices(themePackChoices, defaultSelections, requiredCount);
        }

        public void StartBattle()
        {
            var loadout = new PlayerLoadout
            {
                Transformation = _transformationPicker.SelectedChoice,
                ThemePacks = _brainPacks.SelectedPacks
                    .Concat(_heartPacks.SelectedPacks)
                    .Concat(_subPacks.SelectedPacks)
                    .ToArray(),

                PermanentBuyPile = ContentRegistry.CardPacks["StandardPermanentBuyPile"].Cards.Values,
                StartingDeck = PlayerStartingDeck.StartingDeck(),
            };

            Maps.Instance.GoToBattleScreen(loadout);
        }

        private void RefreshStartButton()
        {
            bool enableStartButton =
                _brainPacks.SelectionsValid &&
                _heartPacks.SelectionsValid &&
                _subPacks.SelectionsValid;

            GetNode<Button>("%StartButton").Disabled = !enableStartButton;
        }
    }
}