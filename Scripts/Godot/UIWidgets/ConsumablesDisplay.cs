using System;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class ConsumablesDisplay : Control
    {
        [Signal] public delegate void ConsumableClickedEventHandler(int index);

        [Export] public bool EnableInput = true;

        public void Refresh(Consumable[] consumables)
        {
            while (GetChildCount() > 0)
            {
                var c = GetChild(0);
                RemoveChild(c);
                c.QueueFree();
            }

            for(int i = 0; i < consumables.Length; i++)
            {
                var button = new Button();
                button.Text = consumables[i].Name;
                AddChild(button);

                // We need to make a copy of this value so it can be used within
                // the closure.  This is because "i" will have changed by the
                // time the potion is clicked, since it's the looping variable.
                int index = i;
                button.Pressed += () =>
                {
                    if (EnableInput)
                        EmitSignal(SignalName.ConsumableClicked, index);
                };
            }
        }
    }
}