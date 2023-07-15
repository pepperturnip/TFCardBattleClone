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

        public override void _Ready()
        {
            _cardRegistry = CreateCardRegistry();
            _loadout = new PlayerLoadout
            {
                CardPacks = new[]
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
                    _cardRegistry.CardPacks["Sex"],
                    _cardRegistry.CardPacks["School"]
                },
                PermanentBuyPile = _cardRegistry.CardPacks["StandardPermanentBuyPile"],
                StartingDeck = PlayerStartingDeck.StartingDeck()
            };

            StartBattle();
        }

        public void StartBattle()
        {
            Maps.Instance.GoToBattleScreen(_loadout, _cardRegistry);
        }

        private CardRegistry CreateCardRegistry()
        {
            var registry = new CardRegistry();

            IEnumerable<string> packNames = DirAccess
                .GetFilesAt("res://CardPacks")
                .Select(f => f.Split(".json")[0]);

            foreach (string packName in packNames)
            {
                string path = $"res://CardPacks/{packName}.json";
                var cards = Core.Parsing.CardPacks.Parse(FileAccess.GetFileAsString(path));

                registry.ImportCardPack(packName, cards);
            }

            return registry;
        }
    }
}