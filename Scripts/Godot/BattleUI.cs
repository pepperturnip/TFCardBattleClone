
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
        private Label _tf => GetNode<Label>("%TFLabel");


        private PlayerHandDisplay _handDisplay => GetNode<PlayerHandDisplay>("%HandDisplay");
        private HBoxContainer _buyPileDisplay => GetNode<HBoxContainer>("%BuyPileDisplay");


        private BattleController Battle;


        public override void _Ready()
        {
            Battle = new BattleController(
                PlaceholderCards.AutoGenerateCatalog(),
                PlaceholderCards.StartingDeck(),
                GetNode<BattleAnimationPlayer>("%BattleAnimationPlayer"),
                new Random((int)DateTimeOffset.Now.ToUnixTimeMilliseconds())
            );

            Battle.StartTurn();
            RefreshDisplay();
        }

        public async void OnEndTurnClicked()
        {
            await Battle.EndTurn();
            RefreshDisplay();
        }

        public void OnPlayCardClicked(int handIndex)
        {
            Battle.PlayCard(handIndex);
            RefreshDisplay();
        }

        public void OnBuyCardClicked(int buyPileIndex)
        {
            Battle.BuyCard(buyPileIndex);
            RefreshDisplay();
        }

        public void RefreshDisplay()
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
            _tf.Text = $"TF: {Battle.State.TF}";

            _handDisplay.Refresh(Battle.State);
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