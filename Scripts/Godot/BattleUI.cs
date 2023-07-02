
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class BattleUI : Control
    {
        [Export] public CardModelFactory CardModelFactory;

        private TFBar _playerTFBar => GetNode<TFBar>("%PlayerTFBar");
        private TFBar _enemyTFBar => GetNode<TFBar>("%EnemyTFBar");

        private Label _enemyDamageRangeLabel => GetNode<Label>("%EnemyDamageRangeLabel");

        private HandDisplay _handDisplay => GetNode<HandDisplay>("%HandDisplay");
        private BuyPileDisplay _buyPileDisplay => GetNode<BuyPileDisplay>("%BuyPileDisplay");

        private ResourcesDisplay _resourcesDisplay => GetNode<ResourcesDisplay>("%ResourcesDisplay");
        private ConsumablesDisplay _consumablesDisplay => GetNode<ConsumablesDisplay>("%ConsumablesDisplay");


        private BattleController Battle;


        public override async void _Ready()
        {
            var cardRegistry = CreateCardRegistry();

            var playerLoadout = new PlayerLoadout
            {
                CardPacks = new[]
                {
                    cardRegistry.CardPacks["Mind"],
                    cardRegistry.CardPacks["Tech"],
                    cardRegistry.CardPacks["Hypno"],
                    cardRegistry.CardPacks["Chemist"],
                    cardRegistry.CardPacks["Ambition"],
                    cardRegistry.CardPacks["Purity"],
                    cardRegistry.CardPacks["Whore"],
                    cardRegistry.CardPacks["FemmeFatale"],
                    cardRegistry.CardPacks["Tease"],
                    cardRegistry.CardPacks["Romance"],
                    cardRegistry.CardPacks["Blowjob"],
                    cardRegistry.CardPacks["Submissive"],
                    cardRegistry.CardPacks["Bondage"],
                    cardRegistry.CardPacks["Cum"],
                    cardRegistry.CardPacks["Cock"],
                    cardRegistry.CardPacks["Sex"]
                },
                PermanentBuyPile = cardRegistry.CardPacks["StandardPermanentBuyPile"],
                StartingDeck = PlayerStartingDeck.StartingDeck()
            };

            Battle = new BattleController(
                loadout: playerLoadout,
                rng: new Random((int)DateTimeOffset.Now.ToUnixTimeMilliseconds()),
                cardRegistry: cardRegistry,
                animationPlayer: GetNode<BattleAnimationPlayer>("%BattleAnimationPlayer")
            );

            this.CardModelFactory.SetBattleState(Battle.State);
            _buyPileDisplay.SetBattleState(Battle.State);
            _handDisplay.SetBattleState(Battle.State);

            await Battle.StartTurn();
            RefreshDisplay();
        }

        public override void _Process(double delta)
        {
            if (Battle.BattleEnded)
                GetTree().ChangeSceneToPacked(Maps.Instance.TitleScreen);
        }

        public void OnIsAnimatingChanged(bool isAnimating) => EnableInput(!isAnimating);

        public async void OnEndTurnClicked()
        {
            await Battle.EndTurn();
            RefreshDisplay();
        }

        public async void OnPlayCardClicked(int handIndex)
        {
            await Battle.PlayCard(handIndex);
            RefreshDisplay();
        }

        public async void OnBuyCardClicked(int buyPileIndex)
        {
            await Battle.BuyCard(buyPileIndex);
            RefreshDisplay();
        }

        public async void OnUseConsumableClicked(int index)
        {
            await Battle.UseConsumable(index);
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            this.CardModelFactory.SetBattleState(Battle.State);

            _enemyDamageRangeLabel.Text = $"{Battle.EnemyMinTFDamage} - {Battle.EnemyMaxTFDamage}";

            _playerTFBar.MaxValue = Battle.State.PlayerMaxTF;
            _playerTFBar.Value = Battle.State.PlayerTF;

            _enemyTFBar.MaxValue = Battle.State.EnemyMaxTF;
            _enemyTFBar.Value = Battle.State.EnemyTF;

            _handDisplay.Refresh();
            _buyPileDisplay.Refresh();

            _resourcesDisplay.UpdateResources(Battle.State);
            _consumablesDisplay.Refresh(Battle.State.Consumables.ToArray());
        }

        private void EnableInput(bool enabled)
        {
            _handDisplay.EnableInput = enabled;
            _buyPileDisplay.EnableInput = enabled;
            GetNode<Button>("%EndTurnButton").Disabled = !enabled;
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