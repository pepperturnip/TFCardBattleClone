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
        private Node2D _cardModelsHolder => GetNode<Node2D>("%CardModels");

        private CardModel[] _cardModels = new CardModel[BattleController.MaxHandSize];
        private Vector2 _cardSize;

        public override void _Ready()
        {
            // Get the size of a card by measuring a dummy one
            var dummyCard = CardModelPrefab.Instantiate<CardModel>();
            _cardSize = dummyCard.Size;
            dummyCard.QueueFree();
            dummyCard = null;

            // Create a pool of card models
            for (int i = 0; i < _cardModels.Length; i++)
            {
                _cardModels[i] = CardModelPrefab.Instantiate<CardModel>();
            }
        }

        public override void _Process(double deltaD)
        {
            float delta = (float)deltaD;

            // Gradually move each card model to its positioner
            for (int i = 0; i < _cardModelsHolder.GetChildCount(); i++)
            {
                var model = _cardModelsHolder.GetChild<Node2D>(i);
                var positioner = _cardPositions.GetChild<CardPositioner>(i);

                model.GlobalPosition = model.GlobalPosition.MoveToward(
                    positioner.PositionNode.GlobalPosition,
                    CardMoveSpeed * delta
                );
            }
        }

        public void Refresh(BattleState state)
        {
            RefreshCardModels(state);
            RefreshCardPositioners(state);
        }

        private void RefreshCardModels(BattleState state)
        {
            for (int i = 0; i < _cardModels.Length; i++)
            {
                var model = _cardModels[i];

                // Add it to the scene tree if it's needed, and remove it if
                // it's not.
                if (i < state.Hand.Count && !_cardModelsHolder.IsAncestorOf(model))
                {
                    _cardModelsHolder.AddChild(model);
                }
                if (i >= state.Hand.Count && _cardModelsHolder.IsAncestorOf(model))
                {
                    _cardModelsHolder.RemoveChild(model);
                }

                // Make it display the correct card
                if (i < state.Hand.Count)
                    model.Card = state.Hand[i];
            }
        }

        private void RefreshCardPositioners(BattleState state)
        {
            var totalSize = new Vector2(
                (_cardSize.X + MinCardSeparation) * state.Hand.Count,
                _cardSize.Y
            );

            while (_cardPositions.GetChildCount() > 0)
            {
                var c = _cardPositions.GetChild(0);
                _cardPositions.RemoveChild(c);
                c.QueueFree();
            }

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