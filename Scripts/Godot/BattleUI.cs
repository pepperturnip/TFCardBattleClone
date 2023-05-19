
using System;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class BattleUI : Control
    {
        [Export] public PackedScene CardDisplayPrefab;

        [Export] public int PlayerTF
        {
            get => Battle.State.PlayerTF;
            set {}
        }

        private HBoxContainer _handDisplay => GetNode<HBoxContainer>("%HandDisplay");

        private BattleController Battle = new BattleController(
            PlaceholderCards.AutoGenerateCatalog(),
            PlaceholderCards.StartingDeck(),
            new Random((int)DateTimeOffset.Now.ToUnixTimeMilliseconds())
        );

        public override void _Ready()
        {
            RefreshHandDisplay();
        }

        public void OnEndTurnClicked()
        {
            Battle.EndTurn();
            RefreshHandDisplay();
        }

        private void RefreshDisplay()
        {
            RefreshHandDisplay();
        }

        private void RefreshHandDisplay()
        {
            while (_handDisplay.GetChildCount() > 0)
                _handDisplay.RemoveChild(_handDisplay.GetChild(0));

            foreach (var card in Battle.State.Hand)
            {
                var cardDisplay = CardDisplayPrefab.Instantiate<CardDisplay>();
                cardDisplay.Card = card;
                _handDisplay.AddChild(cardDisplay);
            }
        }
    }
}