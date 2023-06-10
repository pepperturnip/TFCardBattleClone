using System;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class PlayerHandDisplay : Control
    {
        [Signal] public delegate void CardPlayedEventHandler(int handIndex);

        [Export] public PackedScene CardModelPrefab;
        [Export] public float CardMoveSpeed = 1000;
        [Export] public float MinCardSeparation = 8;

        private Control _cardPositions => GetNode<Control>("%CardPositions");
        private Node2D _cardModels => GetNode<Node2D>("%CardModels");
        private Vector2 _cardSize;

        public override void _Ready()
        {
            // Get the size of a card by measuring a dummy one
            var dummyCard = CardModelPrefab.Instantiate<CardModel>();
            _cardSize = dummyCard.Size;
            dummyCard.QueueFree();
            dummyCard = null;
        }

        public override void _Process(double deltaD)
        {
        }

        public void Refresh(BattleState state)
        {
            RefreshCardPositioners(state);
            RefreshCardModels(state);
        }

        private void RefreshCardPositioners(BattleState state)
        {
            var totalSize = new Vector2(
                (_cardSize.X + MinCardSeparation) * state.Hand.Count,
                _cardSize.Y
            );

            DeleteAllChildren(_cardPositions);

            for(int i = 0; i < state.Hand.Count; i++)
            {
                var cardPositioner = new CardPositioner();
                cardPositioner.Size = _cardSize;
                cardPositioner.CustomMinimumSize = _cardSize;
                _cardPositions.AddChild(cardPositioner);

                cardPositioner.Position = new Vector2(
                    ((_cardSize.X + MinCardSeparation) * i) - (totalSize.X / 2),
                    0
                );

                // We need to make a copy of this value so it can be used within
                // the closure.  This is because "i" will have changed by the
                // time the card is clicked, since it's the looping variable.
                int handIndex = i;
                cardPositioner.Clicked += () => EmitSignal(SignalName.CardPlayed, handIndex);
            }
        }

        private void RefreshCardModels(BattleState state)
        {
            DeleteAllChildren(_cardModels);

            for (int i = 0; i < state.Hand.Count; i++)
            {
                var model = CardModelPrefab.Instantiate<CardModel>();
                model.Card = state.Hand[i];
                _cardModels.AddChild(model);

                // Immediately move it to its position
                model.GlobalPosition = _cardPositions.GetChild<CardPositioner>(i).PositionNode.GlobalPosition;
            }
        }

        private void DeleteAllChildren(Node node)
        {
            while (node.GetChildCount() > 0)
            {
                var c = node.GetChild(0);
                node.RemoveChild(c);
                c.QueueFree();
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