using System;
using TFCardBattle.Core;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class CardButton : Button
    {
        [Export] public float CardHoverScale = 1.1f;
        [Export] public float CardHoverGrowSpeed = 1;

        public CardModel Model => GetNode<CardModel>("%Model");

        private bool _isMouseOver = false;

        public void SetCard(Card card, BattleState state)
        {
            Model.SetCard(card, state);
        }

        public override void _Ready()
        {
            base._Ready();

            MouseEntered += () => _isMouseOver = true;
            MouseExited += () => _isMouseOver = false;
        }

        public override void _Process(double deltaD)
        {
            float delta = (float)deltaD;

            Model.Enabled = !this.Disabled;

            // Make cards bigger when being hovered over
            bool isBig = !Disabled && _isMouseOver;

            var targetScale = isBig
                ? Vector2.One * CardHoverScale
                : Vector2.One;

            Model.CenterScale = Model.CenterScale.MoveToward(targetScale, CardHoverGrowSpeed * delta);
        }
    }
}