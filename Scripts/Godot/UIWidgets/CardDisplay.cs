using System;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class CardDisplay : Control
    {
        [Signal] public delegate void ClickedEventHandler();

        public ICard Card
        {
            get => _model.Card;
            set => _model.Card = value;
        }

        private CardModel _model => GetNode<CardModel>("%CardModel");

        public override void _GuiInput(InputEvent ev)
        {
            if (ev is InputEventMouseButton clickEvent)
                EmitSignal(SignalName.Clicked);
        }
    }
}