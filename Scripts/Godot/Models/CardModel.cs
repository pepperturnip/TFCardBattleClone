using System;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class CardModel : Node2D
    {
        public ICard Card;

        private Label _nameLabel => GetNode<Label>("%NameLabel");
        private Label _descLabel => GetNode<Label>("%DescLabel");
        private CardCostDisplay _costDisplay => GetNode<CardCostDisplay>("%CardCostDisplay");

        public override void _Process(double delta)
        {
            _nameLabel.Text = Card?.Name ?? "null";
            _descLabel.Text = Card?.Desc ?? "";
            _costDisplay.Card = Card;
        }
    }
}