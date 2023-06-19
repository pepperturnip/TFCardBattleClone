
using System;
using System.Threading.Tasks;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class BattleUI : Control
    {
        [Export] public PackedScene CardDisplayPrefab;
        [Export] public CardModelFactory CardModelFactory;

        private TFBar _playerTFBar => GetNode<TFBar>("%PlayerTFBar");
        private TFBar _enemyTFBar => GetNode<TFBar>("%EnemyTFBar");

        private Label _enemyDamageRangeLabel => GetNode<Label>("%EnemyDamageRangeLabel");

        private CardRowDisplay _handDisplay => GetNode<CardRowDisplay>("%HandDisplay");
        private CardRowDisplay _buyPileDisplay => GetNode<CardRowDisplay>("%BuyPileDisplay");

        private ResourcesDisplay _resourcesDisplay => GetNode<ResourcesDisplay>("%ResourcesDisplay");
        private ConsumablesDisplay _consumablesDisplay => GetNode<ConsumablesDisplay>("%ConsumablesDisplay");


        private BattleController Battle;


        public override async void _Ready()
        {
            Battle = new BattleController(
                new PlayerLoadout
                {
                    CardPacks = new[]
                    {
                        CardPacks.Load("PlaceholderCardPack"),
                        CardPacks.Load("Mind"),
                        CardPacks.Load("Tech"),
                        CardPacks.Load("Hypno"),
                        CardPacks.Load("Chemist"),
                        CardPacks.Load("Ambition"),
                        CardPacks.Load("Purity"),
                        CardPacks.Load("Whore")
                    },
                    PermanentBuyPile = CardPacks.Load("PlaceholderPermanentBuyPile"),
                    StartingDeck = PlayerStartingDeck.StartingDeck()
                },
                GetNode<BattleAnimationPlayer>("%BattleAnimationPlayer"),
                new Random((int)DateTimeOffset.Now.ToUnixTimeMilliseconds())
            );

            this.CardModelFactory.SetBattleState(Battle.State);

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

            _handDisplay.Refresh(Battle.State.Hand.ToArray());
            _buyPileDisplay.Refresh(Battle.State.BuyPile.ToArray());

            _resourcesDisplay.UpdateResources(Battle.State);
            _consumablesDisplay.Refresh(Battle.State.Consumables.ToArray());
        }

        private void EnableInput(bool enabled)
        {
            _handDisplay.EnableInput = enabled;
            _buyPileDisplay.EnableInput = enabled;
            GetNode<Button>("%EndTurnButton").Disabled = !enabled;
        }
    }
}