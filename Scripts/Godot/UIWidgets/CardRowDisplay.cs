using System;
using System.Linq;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class CardRowDisplay : Control
    {
        [Signal] public delegate void CardPlayedEventHandler(int handIndex);

        [Export] public PackedScene CardModelPrefab;
        [Export] public float MinCardSeparation = 8;
        [Export] public float CardMoveDecayRate = 10;

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

        public override void _Process(double delta)
        {
            // Exponentially decay all card models to their target positions
            for (int i = 0; i < _cardModels.GetChildCount(); i++)
            {
                var card = _cardModels.GetChild<CardModel>(i);
                var targetPos = TargetGlobalPosition(i, _cardModels.GetChildCount());

                float dist = card.GlobalPosition.DistanceTo(targetPos);
                float newDist = dist * Mathf.Pow(Mathf.E, -CardMoveDecayRate * (float)delta);
                float deltaPos = Mathf.Abs(dist - newDist);

                card.GlobalPosition = card.GlobalPosition.MoveToward(targetPos, deltaPos);
            }
        }

        public void DrawCard(ICard[] newHand)
        {
            RefreshCardPositioners(newHand);

            // Spawn a model for the new card at the deck position.
            var newestModel = CardModelPrefab.Instantiate<CardModel>();
            newestModel.Card = newHand.Last();
            _cardModels.AddChild(newestModel);
            newestModel.GlobalPosition = Vector2.Zero;
        }

        public void RemoveCard(int removedHandIndex, ICard[] newHand)
        {
            RefreshCardPositioners(newHand);

            var modelToRemove = _cardModels.GetChild<CardModel>(removedHandIndex);
            _cardModels.RemoveChild(modelToRemove);
            modelToRemove.QueueFree();
        }

        public void Refresh(ICard[] hand)
        {
            // HACK: Skip refreshing if nothing changed, to prevent the card
            // move animation from being needlessly interrupted.
            var oldHand = _cardModels
                .EnumerateChildren<CardModel>()
                .Select(m => m.Card);

            if (hand.SequenceEqual(oldHand))
                return;

            GD.Print($"Forcibly refreshing hand display({_forceRefreshCount++})");

            RefreshCardPositioners(hand);
            RefreshCardModels(hand);
        }
        private int _forceRefreshCount = 0;

        private void RefreshCardPositioners(ICard[] hand)
        {
            var totalSize = new Vector2(
                (_cardSize.X + MinCardSeparation) * hand.Length,
                _cardSize.Y
            );

            DeleteAllChildren(_cardPositions);

            for(int i = 0; i < hand.Length; i++)
            {
                var cardPositioner = new CardPositioner();
                cardPositioner.Size = _cardSize;
                cardPositioner.CustomMinimumSize = _cardSize;
                _cardPositions.AddChild(cardPositioner);

                cardPositioner.GlobalPosition = TargetGlobalPosition(i, hand.Length);

                // We need to make a copy of this value so it can be used within
                // the closure.  This is because "i" will have changed by the
                // time the card is clicked, since it's the looping variable.
                int handIndex = i;
                cardPositioner.Clicked += () => EmitSignal(SignalName.CardPlayed, handIndex);
            }
        }

        private void RefreshCardModels(ICard[] hand)
        {
            DeleteAllChildren(_cardModels);

            for (int i = 0; i < hand.Length; i++)
            {
                var model = CardModelPrefab.Instantiate<CardModel>();
                model.Card = hand[i];
                _cardModels.AddChild(model);

                // Immediately move it to its position
                model.GlobalPosition = TargetGlobalPosition(i, hand.Length);
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

        private Vector2 TargetGlobalPosition(int handIndex, int handCount)
        {
            var totalSize = new Vector2(
                (_cardSize.X + MinCardSeparation) * handCount,
                _cardSize.Y
            );

            float x = (_cardSize.X + MinCardSeparation) * handIndex;
            x -= totalSize.X / 2;

            return _cardPositions.GlobalPosition + Vector2.Right * x;
        }

        private partial class CardPositioner : Control
        {
            [Signal] public delegate void ClickedEventHandler();

            public override void _GuiInput(InputEvent ev)
            {
                if (ev is InputEventMouseButton clickEvent)
                    EmitSignal(SignalName.Clicked);
            }
        }
    }
}