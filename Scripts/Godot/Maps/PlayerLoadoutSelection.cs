using System;
using System.Collections.Generic;
using System.Linq;
using TFCardBattle.Core;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class PlayerLoadoutSelection : Control
    {
        private ContentRegistry _registry;
        private PlayerLoadout _loadout;

        private Transformation[] _transformationChoices;
        private ItemList _transformationPicker => GetNode<ItemList>("%TransformationPicker");

        public override void _Ready()
        {
            _registry = CreateContentRegistry();
            _transformationChoices = _registry.Transformations.Values.ToArray();

            _loadout = new PlayerLoadout(_registry)
            {
                Transformation = _transformationChoices[0],
                PermanentBuyPile = _registry.CardPacks["StandardPermanentBuyPile"],
                StartingDeck = PlayerStartingDeck.StartingDeck(),

                ThemePacks = new[]
                {
                    _registry.CardPacks["Tech"],
                    _registry.CardPacks["Hypno"],
                    _registry.CardPacks["Chemist"],
                    _registry.CardPacks["Ambition"],
                    _registry.CardPacks["Purity"],
                    _registry.CardPacks["FemmeFatale"],
                    _registry.CardPacks["Tease"],
                    _registry.CardPacks["Romance"],
                    _registry.CardPacks["Blowjob"],
                    _registry.CardPacks["Bondage"],
                    _registry.CardPacks["Cum"],
                    _registry.CardPacks["Cock"],
                    _registry.CardPacks["Sex"]
                }
            };

            foreach (var tf in _transformationChoices)
            {
                _transformationPicker.AddItem(tf.Name);
            }
            _transformationPicker.Select(0);
            _transformationPicker.ItemSelected += i => _loadout.Transformation = _transformationChoices[i];
        }

        public void StartBattle()
        {
            Maps.Instance.GoToBattleScreen(_loadout, _registry);
        }

        private static ContentRegistry CreateContentRegistry()
        {
            var registry = new ContentRegistry();

            foreach (string packId in IdsInFolder("res://Content/CardPacks"))
            {
                string path = $"res://Content/CardPacks/{packId}.json";
                registry.ImportCardPackOld(packId, FileAccess.GetFileAsString(path));
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