using System;
using TFCardBattle.Core;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class HoverGrowButton : Button
    {
        [Export] public float HoverScale = 1.1f;
        [Export] public float HoverGrowSpeed = 1;

        [Export] public Color EnabledModulate = Colors.White;
        [Export] public Color DisabledModulate = Color.FromHsv(0, 0, 0.5f);

        public Control Model => GetChild<Control>(0);

        private bool _isMouseOver = false;

        public override void _Ready()
        {
            base._Ready();

            MouseEntered += () => _isMouseOver = true;
            MouseExited += () => _isMouseOver = false;
        }

        public override void _Process(double deltaD)
        {
            float delta = (float)deltaD;

            Model.MouseFilter = MouseFilterEnum.Ignore;

            Model.Modulate = this.Disabled
                ? DisabledModulate
                : EnabledModulate;

            // Make it bigger when hovered over
            bool isBig = !Disabled && _isMouseOver;

            var targetScale = isBig
                ? Vector2.One * HoverScale
                : Vector2.One;

            Model.PivotOffset = Model.Size / 2;
            Model.Scale = Model.Scale.MoveToward(targetScale, HoverGrowSpeed * delta);
        }
    }
}