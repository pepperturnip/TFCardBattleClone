using System;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class CardDisplay : Control
    {
        public ICard Card;

        [Export] public string CardName
        {
            get => Card?.Name ?? "null";
            set {}
        }
    }
}