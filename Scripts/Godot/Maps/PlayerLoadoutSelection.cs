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
        private ContentRegistry _registry;

        private Transformation[] _transformationChoices;
        private CardPack[] _themePackChoices;

        private ItemList _transformationPicker => GetNode<ItemList>("%TransformationPicker");
        private VBoxContainer _themePackPicker => GetNode<VBoxContainer>("%ThemePackPicker");

        private HashSet<CardPack> _selectedThemePacks = new HashSet<CardPack>();


        public override void _Ready()
        {
            _registry = CreateContentRegistry();

            InitTransformationPicker();
            InitThemePackPicker();
        }

        private void InitTransformationPicker()
        {
            _transformationChoices = _registry.Transformations.Values.ToArray();

            foreach (var tf in _transformationChoices)
            {
                _transformationPicker.AddItem(tf.Name);
            }

            _transformationPicker.Select(0);
        }

        private void InitThemePackPicker()
        {
            _themePackChoices = _registry.CardPacks
                .Values
                .Where(p => p.Type == CardPackType.Standard)
                .ToArray();

            string defaultSelectionsJson = FileAccess.GetFileAsString("res://Content/DefaultLoadout.json");
            var defaultSelections = JsonConvert.DeserializeObject<CardPackId[]>(defaultSelectionsJson)
                .Select(id => _registry.CardPacks[id])
                .ToHashSet();

            for (int i = 0; i < _themePackChoices.Length; i++)
            {
                var cardPack = _themePackChoices[i];

                var checkBox = new CheckBox();
                checkBox.Text = cardPack.Name;
                checkBox.ButtonPressed = defaultSelections.Contains(cardPack);
                checkBox.Toggled += (bool pressed) =>
                {
                    if (pressed)
                        _selectedThemePacks.Add(cardPack);
                    else
                        _selectedThemePacks.Remove(cardPack);
                };

                _themePackPicker.AddChild(checkBox);
            }
        }

        public void StartBattle()
        {
            var loadout = new PlayerLoadout(_registry)
            {
                Transformation = _transformationChoices[_transformationPicker.GetSelectedItems()[0]],
                ThemePacks = _selectedThemePacks.ToArray(),

                PermanentBuyPile = _registry.CardPacks["StandardPermanentBuyPile"].Cards.Values,
                StartingDeck = PlayerStartingDeck.StartingDeck(),
            };

            Maps.Instance.GoToBattleScreen(loadout, _registry);
        }

        private static ContentRegistry CreateContentRegistry()
        {
            var registry = new ContentRegistry();

            foreach (string packId in IdsInFolder("res://Content/CardPacks"))
            {
                string path = $"res://Content/CardPacks/{packId}.json";
                registry.ImportCardPack(packId, FileAccess.GetFileAsString(path));
            }

            foreach (string fileNameWithoutExt in IdsInFolder("res://Content/ConsumablePacks"))
            {
                string path = $"res://Content/ConsumablePacks/{fileNameWithoutExt}.json";
                registry.ImportConsumables(FileAccess.GetFileAsString(path));
            }

            foreach (string tfId in IdsInFolder("res://Content/Transformations"))
            {
                string path = $"res://Content/Transformations/{tfId}.json";
                registry.ImportTransformation(tfId, FileAccess.GetFileAsString(path));
            }

            return registry;

            IEnumerable<string> IdsInFolder(string folder)
            {
                return DirAccess
                    .GetFilesAt(folder)
                    .Select(f => f.Split(".json")[0]);
            }
        }
    }
}