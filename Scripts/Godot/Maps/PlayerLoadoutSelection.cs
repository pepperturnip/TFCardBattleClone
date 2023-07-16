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
            _transformationChoices = CreateTransformations(_registry);

            _loadout = new PlayerLoadout(_registry)
            {
                Transformation = _transformationChoices[0],
                PermanentBuyPile = _registry.CardPacks["StandardPermanentBuyPile"],
                StartingDeck = PlayerStartingDeck.StartingDeck(),

                ThemePacks = new[]
                {
                    _registry.CardPacks["Mind"],
                    _registry.CardPacks["Tech"],
                    _registry.CardPacks["Hypno"],
                    _registry.CardPacks["Chemist"],
                    _registry.CardPacks["Ambition"],
                    _registry.CardPacks["Purity"],
                    _registry.CardPacks["Whore"],
                    _registry.CardPacks["FemmeFatale"],
                    _registry.CardPacks["Tease"],
                    _registry.CardPacks["Romance"],
                    _registry.CardPacks["Blowjob"],
                    _registry.CardPacks["Submissive"],
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

        private static Transformation[] CreateTransformations(ContentRegistry registry)
        {
            return new[]
            {
                new Transformation
                {
                    Name = "Futanari",
                    RequiredCardPacks = new CardPackId[] {"Futanari"}
                },

                new Transformation
                {
                    Name = "Schoolgirl",
                    RequiredCardPacks = new CardPackId[] {"School"}
                }
            };
        }

        private static ContentRegistry CreateContentRegistry()
        {
            var registry = new ContentRegistry();

            IEnumerable<string> packNames = DirAccess
                .GetFilesAt("res://CardPacks")
                .Select(f => f.Split(".json")[0]);

            foreach (string packName in packNames)
            {
                string path = $"res://CardPacks/{packName}.json";
                registry.ImportCardPack(packName, FileAccess.GetFileAsString(path));
            }

            return registry;
        }
    }
}