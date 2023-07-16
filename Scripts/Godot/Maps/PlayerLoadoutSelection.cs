using System;
using System.Collections.Generic;
using System.Linq;
using TFCardBattle.Core;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class PlayerLoadoutSelection : Control
    {
        private CardRegistry _cardRegistry;
        private PlayerLoadout _loadout;

        private Transformation[] _transformationChoices;
        private ItemList _transformationPicker => GetNode<ItemList>("%TransformationPicker");

        public override void _Ready()
        {
            _cardRegistry = CreateCardRegistry();
            _transformationChoices = CreateTransformations(_cardRegistry);

            _loadout = new PlayerLoadout
            {
                Transformation = _transformationChoices[0],
                PermanentBuyPile = _cardRegistry.CardPacks["StandardPermanentBuyPile"],
                StartingDeck = PlayerStartingDeck.StartingDeck(),

                ThemePacks = new[]
                {
                    _cardRegistry.CardPacks["Mind"],
                    _cardRegistry.CardPacks["Tech"],
                    _cardRegistry.CardPacks["Hypno"],
                    _cardRegistry.CardPacks["Chemist"],
                    _cardRegistry.CardPacks["Ambition"],
                    _cardRegistry.CardPacks["Purity"],
                    _cardRegistry.CardPacks["Whore"],
                    _cardRegistry.CardPacks["FemmeFatale"],
                    _cardRegistry.CardPacks["Tease"],
                    _cardRegistry.CardPacks["Romance"],
                    _cardRegistry.CardPacks["Blowjob"],
                    _cardRegistry.CardPacks["Submissive"],
                    _cardRegistry.CardPacks["Bondage"],
                    _cardRegistry.CardPacks["Cum"],
                    _cardRegistry.CardPacks["Cock"],
                    _cardRegistry.CardPacks["Sex"]
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
            Maps.Instance.GoToBattleScreen(_loadout, _cardRegistry);
        }

        private static Transformation[] CreateTransformations(CardRegistry cardRegistry)
        {
            return new[]
            {
                new Transformation
                {
                    Name = "Futanari",
                    CardPack = cardRegistry.CardPacks["Futanari"],
                },

                new Transformation
                {
                    Name = "Schoolgirl",
                    CardPack = cardRegistry.CardPacks["School"],
                }
            };
        }

        private static CardRegistry CreateCardRegistry()
        {
            var registry = new CardRegistry();

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