
using System;
using System.Threading.Tasks;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class BattleUI : Control
    {
        [Export] public PackedScene CardDisplayPrefab;

        private Label _playerTFLabel => GetNode<Label>("%PlayerTFLabel");
        private ProgressBar _playerTFBar => GetNode<ProgressBar>("%PlayerTFBar");

        private Label _enemyTFLabel => GetNode<Label>("%EnemyTFLabel");
        private ProgressBar _enemyTFBar => GetNode<ProgressBar>("%EnemyTFBar");


        private Label _brain => GetNode<Label>("%BrainLabel");
        private Label _heart => GetNode<Label>("%HeartLabel");
        private Label _sub => GetNode<Label>("%SubLabel");
        private Label _shield => GetNode<Label>("%ShieldLabel");
        private Label _damage => GetNode<Label>("%DamageResourceLabel");


        private PlayerHandDisplay _handDisplay => GetNode<PlayerHandDisplay>("%HandDisplay");
        private HBoxContainer _buyPileDisplay => GetNode<HBoxContainer>("%BuyPileDisplay");


        private BattleController Battle;


        public override async void _Ready()
        {
            Battle = new BattleController(
                PlaceholderCards.AutoGenerateCatalog(),
                PlaceholderCards.StartingDeck(),
                GetNode<BattleAnimationPlayer>("%BattleAnimationPlayer"),
                new Random((int)DateTimeOffset.Now.ToUnixTimeMilliseconds())
            );

            await Battle.StartTurn();
            RefreshDisplay();
        }

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

        private void RefreshDisplay()
        {
            _playerTFBar.MaxValue = Battle.State.PlayerMaxTF;
            _playerTFBar.Value = Battle.State.PlayerTF;
            _playerTFLabel.Text = $"{Battle.State.PlayerTF} / {Battle.State.PlayerMaxTF}";

            _enemyTFBar.MaxValue = Battle.State.EnemyMaxTF;
            _enemyTFBar.Value = Battle.State.EnemyTF;
            _enemyTFLabel.Text = $"{Battle.State.EnemyTF} / {Battle.State.EnemyMaxTF}";

            _brain.Text = $"Brain: {Battle.State.Brain}";
            _heart.Text = $"Heart: {Battle.State.Heart}";
            _sub.Text = $"Sub: {Battle.State.Sub}";
            _shield.Text = $"Shield: {Battle.State.Shield}";
            _damage.Text = $"Damage: {Battle.State.Damage}";

            _handDisplay.Refresh(Battle.State.Hand.ToArray());
            RefreshBuyPileDisplay();
        }

        private void RefreshBuyPileDisplay()
        {
            while (_buyPileDisplay.GetChildCount() > 0)
            {
                var c = _buyPileDisplay.GetChild(0);
                _buyPileDisplay.RemoveChild(c);
                c.QueueFree();
            }

            for(int i = 0; i < Battle.State.BuyPile.Count; i++)
            {
                var card = Battle.State.BuyPile[i];
                var cardDisplay = CardDisplayPrefab.Instantiate<CardDisplay>();
                cardDisplay.Card = card;
                _buyPileDisplay.AddChild(cardDisplay);

                int buyPileIndex = i;
                cardDisplay.Clicked += () => OnBuyCardClicked(buyPileIndex);
            }
        }
    }
}