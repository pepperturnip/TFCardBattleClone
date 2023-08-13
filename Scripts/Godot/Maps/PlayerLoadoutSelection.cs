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
        private ThemePackPicker _themePackPicker => GetNode<ThemePackPicker>("%ThemePackPicker");

        public override void _Ready()
        {
            InitTransformationPicker();
            InitThemePackPicker();
        }

        private void InitTransformationPicker()
        {
            var choices = ContentRegistry.Transformations.Values.ToArray();
            _transformationPicker.SetChoices(choices);
        }

        private void InitThemePackPicker()
        {
            var brainChoices = ContentRegistry.CardPacks
                .Values
                .Where(p => p.Type == CardPackType.BrainSlot)
                .ToArray();

            var heartChoices = ContentRegistry.CardPacks
                .Values
                .Where(p => p.Type == CardPackType.HeartSlot)
                .ToArray();

            var subChoices = ContentRegistry.CardPacks
                .Values
                .Where(p => p.Type == CardPackType.SubSlot)
                .ToArray();

            _themePackPicker.SetChoices(brainChoices, heartChoices, subChoices);
        }

        public void StartBattle()
        {
            var loadout = new PlayerLoadout
            {
                Transformation = _transformationPicker.SelectedChoice,
                ThemePacks = _themePackPicker.SelectedPacks.ToArray(),

                PermanentBuyPile = ContentRegistry.CardPacks["StandardPermanentBuyPile"].Cards.Values,
                StartingDeck = PlayerStartingDeck.StartingDeck(),
            };

            Maps.Instance.GoToBattleScreen(loadout);
        }

        private void RefreshStartButton()
        {
            GetNode<Button>("%StartButton").Disabled = !_themePackPicker.SelectionsValid;
        }
    }
}