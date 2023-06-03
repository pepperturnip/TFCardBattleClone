using System;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class PlayerHandDisplay : Control
    {
        [Signal] public delegate void CardPlayedEventHandler(int handIndex);

        [Export] public PackedScene CardDisplayPrefab;

        public void Refresh(BattleState state)
        {
            while (GetChildCount() > 0)
            {
                var c = GetChild(0);
                RemoveChild(c);
                c.QueueFree();
            }

            for(int i = 0; i < state.Hand.Count; i++)
            {
                var card = state.Hand[i];
                var cardDisplay = CardDisplayPrefab.Instantiate<CardDisplay>();
                cardDisplay.Card = card;
                AddChild(cardDisplay);

                int handIndex = i;
                cardDisplay.Clicked += () => EmitSignal(SignalName.CardPlayed, handIndex);
            }
        }
    }
}