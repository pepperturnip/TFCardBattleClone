using System;
using System.Linq;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class CardRowDisplay : Control
    {
        [Signal] public delegate void CardClickedEventHandler(int handIndex);

        [Export] public PackedScene CardModelPrefab;
        [Export] public float MinCardSeparation = 8;
        [Export] public float CardMoveDecayRate = 10;
        [Export] public float CardHoverGrowSpeed = 1;
        [Export] public float CardHoverScale = 1.1f;
        [Export] public bool EnableInput = true;

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
            float delta = (float)deltaD;

            for (int i = 0; i < _cardModels.GetChildCount(); i++)
            {
                // Exponentially decay all card models to their target positions
                var card = _cardModels.GetChild<CardModel>(i);
                var targetPos = TargetGlobalPosition(i, _cardModels.GetChildCount());

                float dist = card.GlobalPosition.DistanceTo(targetPos);
                float newDist = dist * Mathf.Pow(Mathf.E, -CardMoveDecayRate * delta);
                float deltaPos = Mathf.Abs(dist - newDist);

                card.GlobalPosition = card.GlobalPosition.MoveToward(targetPos, deltaPos);

                // Make cards bigger when being hovered over
                var targetScale = (_cardPositions.GetChild<CardPositioner>(i).IsMouseOver && EnableInput)
                    ? Vector2.One * CardHoverScale
                    : Vector2.One;

                card.Scale = card.Scale.MoveToward(targetScale, CardHoverGrowSpeed * delta);
            }
        }

        public void AddCard(ICard card)
        {
            var newestModel = CardModelPrefab.Instantiate<CardModel>();
            newestModel.Card = card;
            _cardModels.AddChild(newestModel);
            newestModel.GlobalPosition = Vector2.Zero;

            RefreshCardPositioners(_cardModels.GetChildCount());
        }

        public void RemoveCard(int removedHandIndex)
        {
            var modelToRemove = _cardModels.GetChild<CardModel>(removedHandIndex);
            _cardModels.RemoveChild(modelToRemove);
            modelToRemove.QueueFree();

            RefreshCardPositioners(_cardModels.GetChildCount());
        }

        public void Refresh(ICard[] cards)
        {
            // HACK: Skip refreshing if nothing changed, to prevent the card
            // move animation from being needlessly interrupted.
            var oldCards = _cardModels
                .EnumerateChildren<CardModel>()
                .Select(m => m.Card);

            if (cards.SequenceEqual(oldCards))
                return;

            GD.Print($"Forcibly refreshing card row display({_forceRefreshCount++})");

            RefreshCardPositioners(cards.Length);
            RefreshCardModels(cards);
        }
        private int _forceRefreshCount = 0;

        private void RefreshCardPositioners(int cardCount)
        {
            DeleteAllChildren(_cardPositions);

            for(int i = 0; i < cardCount; i++)
            {
                var cardPositioner = new CardPositioner();
                cardPositioner.Size = _cardSize;
                cardPositioner.CustomMinimumSize = _cardSize;
                _cardPositions.AddChild(cardPositioner);

                cardPositioner.GlobalPosition = TargetGlobalPosition(i, cardCount);

                // We need to make a copy of this value so it can be used within
                // the closure.  This is because "i" will have changed by the
                // time the card is clicked, since it's the looping variable.
                int handIndex = i;
                cardPositioner.Clicked += () =>
                {
                    if (EnableInput)
                        EmitSignal(SignalName.CardClicked, handIndex);
                };
            }
        }

        private void RefreshCardModels(ICard[] cards)
        {
            DeleteAllChildren(_cardModels);

            for (int i = 0; i < cards.Length; i++)
            {
                var model = CardModelPrefab.Instantiate<CardModel>();
                model.Card = cards[i];
                _cardModels.AddChild(model);

                // Immediately move it to its position
                model.GlobalPosition = TargetGlobalPosition(i, cards.Length);
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

            public bool IsMouseOver {get; private set;}

            public override void _Ready()
            {
                base._Ready();

                MouseEntered += () => IsMouseOver = true;
                MouseExited += () => IsMouseOver = false;
            }

            public override void _GuiInput(InputEvent ev)
            {
                if (ev is InputEventMouseButton clickEvent)
                    EmitSignal(SignalName.Clicked);
            }
        }
    }
}