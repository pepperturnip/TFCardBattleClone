
using System;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class BattleUI : Control
    {
        [Export] public PackedScene CardDisplayPrefab;

        private Label _playerTF => GetNode<Label>("%PlayerTF");
        private Label _enemyTF => GetNode<Label>("%EnemyTF");
        private Label _brain => GetNode<Label>("%BrainLabel");
        private Label _heart => GetNode<Label>("%HeartLabel");
        private Label _sub => GetNode<Label>("%SubLabel");
        private Label _shield => GetNode<Label>("%ShieldLabel");
        private Label _tf => GetNode<Label>("%TFLabel");

        private HBoxContainer _handDisplay => GetNode<HBoxContainer>("%HandDisplay");

        private BattleController Battle = new BattleController(
            PlaceholderCards.AutoGenerateCatalog(),
            PlaceholderCards.StartingDeck(),
            new Random((int)DateTimeOffset.Now.ToUnixTimeMilliseconds())
        );

        public override void _Ready()
        {
            Battle.StartTurn();
            RefreshDisplay();
        }

        public void OnEndTurnClicked()
        {
            Battle.EndTurn();
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            _playerTF.Text = $"Player TF: {Battle.State.PlayerTF} / {Battle.State.PlayerMaxTF}";
            _enemyTF.Text = $"Enemy TF: {Battle.State.EnemyTF} / {Battle.State.EnemyMaxTF}";

            _brain.Text = $"Brain: {Battle.State.Brain}";
            _heart.Text = $"Heart: {Battle.State.Heart}";
            _sub.Text = $"Sub: {Battle.State.Subs}";
            _shield.Text = $"Shield: {Battle.State.Shield}";
            _tf.Text = $"TF: {Battle.State.TF}";

            RefreshHandDisplay();
        }

        private void RefreshHandDisplay()
        {
            while (_handDisplay.GetChildCount() > 0)
            {
                var c = _handDisplay.GetChild(0);
                _handDisplay.RemoveChild(c);
                c.QueueFree();
            }

            foreach (var card in Battle.State.Hand)
            {
                var cardDisplay = CardDisplayPrefab.Instantiate<CardDisplay>();
                cardDisplay.Card = card;
                _handDisplay.AddChild(cardDisplay);
            }
        }
    }
}