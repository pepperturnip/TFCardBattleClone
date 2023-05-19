using System;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class CardDisplay : Control
    {
        [Signal] public delegate void ClickedEventHandler();

        public ICard Card;

        [Export] public string CardName
        {
            get => Card?.Name ?? "null";
            set {}
        }

        public override void _GuiInput(InputEvent ev)
        {
            if (ev is InputEventMouseButton clickEvent)
                EmitSignal(SignalName.Clicked);
        }
    }
}