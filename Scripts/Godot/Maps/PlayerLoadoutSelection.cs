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
        private Transformation[] _transformationChoices;

        private ItemList _transformationPicker => GetNode<ItemList>("%TransformationPicker");
        private ThemePackPicker _brainPacks => GetNode<ThemePackPicker>("%BrainPacks");
        private ThemePackPicker _heartPacks => GetNode<ThemePackPicker>("%HeartPacks");
        private ThemePackPicker _subPacks => GetNode<ThemePackPicker>("%SubPacks");

        private const int RequiredBrainCount = 5;
        private const int RequiredHeartCount = 4;
        private const int RequiredSubCount = 4;

        public override void _Ready()
        {
            CreateContentRegistry();

            InitTransformationPicker();
            InitThemePackPicker(_brainPacks, CardPackType.BrainSlot, RequiredBrainCount);
            InitThemePackPicker(_heartPacks, CardPackType.HeartSlot, RequiredHeartCount);
            InitThemePackPicker(_subPacks, CardPackType.SubSlot, RequiredSubCount);
        }

        private void InitTransformationPicker()
        {
            _transformationChoices = ContentRegistry.Transformations.Values.ToArray();

            foreach (var tf in _transformationChoices)
            {
                _transformationPicker.AddItem(tf.Name);
            }

            _transformationPicker.Select(0);
        }

        private void InitThemePackPicker(
            ThemePackPicker picker,
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
                Transformation = _transformationChoices[_transformationPicker.GetSelectedItems()[0]],
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

        private static void CreateContentRegistry()
        {
            foreach (string packId in IdsInFolder("res://Content/CardPacks"))
            {
                string path = $"res://Content/CardPacks/{packId}.json";
                ContentRegistry.ImportCardPack(packId, FileAccess.GetFileAsString(path));
            }

            foreach (string fileNameWithoutExt in IdsInFolder("res://Content/ConsumablePacks"))
            {
                string path = $"res://Content/ConsumablePacks/{fileNameWithoutExt}.json";
                ContentRegistry.ImportConsumables(FileAccess.GetFileAsString(path));
            }

            foreach (string tfId in IdsInFolder("res://Content/Transformations"))
            {
                string path = $"res://Content/Transformations/{tfId}.json";
                ContentRegistry.ImportTransformation(tfId, FileAccess.GetFileAsString(path));
            }

            foreach (string path in FilePathsInFolder("res://Content/CustomResourcePacks"))
            {
                ContentRegistry.ImportCustomResources(FileAccess.GetFileAsString(path));
            }

            IEnumerable<string> IdsInFolder(string folder)
            {
                return DirAccess
                    .GetFilesAt(folder)
                    .Select(f => f.Split(".json")[0]);
            }

            IEnumerable<string> FilePathsInFolder(string folder)
            {
                return DirAccess
                    .GetFilesAt(folder)
                    .Select(f => $"{folder}/{f}");
            }
        }
    }
}