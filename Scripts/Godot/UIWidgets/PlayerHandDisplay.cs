using System;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class PlayerHandDisplay : Control
    {
        [Signal] public delegate void CardPlayedEventHandler(int handIndex);

        [Export] public PackedScene CardModelPrefab;

        private HBoxContainer _cardPositions => GetNode<HBoxContainer>("%CardPositions");
        private Node2D _cardModels => GetNode<Node2D>("%CardModels");

        public override void _Process(double delta)
        {
            for (int i = 0; i < _cardModels.GetChildCount(); i++)
            {
                var model = _cardModels.GetChild<Node2D>(i);
                var positioner = _cardPositions.GetChild<CardPositioner>(i);

                model.GlobalPosition = positioner.GlobalPosition;
            }
        }

        public void Refresh(BattleState state)
        {
            while (_cardPositions.GetChildCount() > 0)
            {
                var c = _cardPositions.GetChild(0);
                _cardPositions.RemoveChild(c);
                c.QueueFree();
            }

            while (_cardModels.GetChildCount() > 0)
            {
                var c = _cardModels.GetChild(0);
                _cardModels.RemoveChild(c);
                c.QueueFree();
            }

            for(int i = 0; i < state.Hand.Count; i++)
            {
                var cardModel = CardModelPrefab.Instantiate<CardModel>();
                cardModel.Card = state.Hand[i];
                _cardModels.AddChild(cardModel);

                var cardPositioner = new CardPositioner();
                cardPositioner.Size = cardModel.Size;
                cardPositioner.CustomMinimumSize = cardModel.Size;
                _cardPositions.AddChild(cardPositioner);

                // We need to make a copy of this value so it can be used within
                // the closure.  This is because "i" will have changed by the
                // time the card is clicked, since it's the looping variable.
                int handIndex = i;
                cardPositioner.Clicked += () => EmitSignal(SignalName.CardPlayed, handIndex);
            }
        }

        private partial class CardPositioner : Control
        {
            [Signal] public delegate void ClickedEventHandler();

            public readonly Node2D PositionNode;

            public CardPositioner()
            {
                PositionNode = new Node2D();
                AddChild(PositionNode);
            }

            public override void _GuiInput(InputEvent ev)
            {
                if (ev is InputEventMouseButton clickEvent)
                    EmitSignal(SignalName.Clicked);
            }
        }
    }
}