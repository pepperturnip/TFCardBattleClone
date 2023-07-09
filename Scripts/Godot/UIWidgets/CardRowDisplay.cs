using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    [Tool]
    public partial class CardRowDisplay : Control
    {
        [Signal] public delegate void CardClickedEventHandler(int handIndex);

        [Export] public PackedScene DummyCardModelPrefab;
        [Export] public CardModelFactory ModelFactory;
        [Export] public float MinCardSeparation = 8;
        [Export] public float CardMoveDecayRate = 10;
        [Export] public float CardHoverGrowSpeed = 1;
        [Export] public float CardHoverScale = 1.1f;
        [Export] public bool EnableInput = true;

        [Export] public int EditorPreviewCardCount = 6;

        public Vector2 CardSize {get; private set;}

        private Control _cardPositions => GetNode<Control>("%CardPositions");
        private Node2D _cardModels => GetNode<Node2D>("%CardModels");

        public override void _Draw()
        {
            if (!Engine.IsEditorHint())
                return;

            for (int i = 0; i < EditorPreviewCardCount; i++)
            {
                var globalPos = TargetGlobalPosition(i, EditorPreviewCardCount);
                var localPos = _cardModels.ToLocal(globalPos);

                DrawRect(
                    rect: new Rect2(localPos, CardSize),
                    color: Colors.Red
                );

                DrawCircle(localPos, 8, Colors.Blue);
            }
        }

        public override void _Ready()
        {
            // Get the size of a card by measuring a dummy one
            var dummyCard = DummyCardModelPrefab.Instantiate<CardModel>();
            CardSize = dummyCard.Size;
            dummyCard.QueueFree();
            dummyCard = null;

            QueueRedraw();
        }

        public override void _Process(double deltaD)
        {
            float delta = (float)deltaD;

            if (Engine.IsEditorHint())
                return;

            for (int i = 0; i < _cardModels.GetChildCount(); i++)
            {
                // Exponentially decay all card models to their target positions
                var model = _cardModels.GetChild<CardModel>(i);
                var targetPos = TargetGlobalPosition(i, _cardModels.GetChildCount());

                float dist = model.GlobalPosition.DistanceTo(targetPos);
                float newDist = dist * Mathf.Pow(Mathf.E, -CardMoveDecayRate * delta);
                float deltaPos = Mathf.Abs(dist - newDist);

                model.GlobalPosition = model.GlobalPosition.MoveToward(targetPos, deltaPos);

                // Make cards bigger when being hovered over
                bool isBig =
                    EnableInput &&
                    model.Enabled &&
                    _cardPositions.GetChild<CardPositioner>(i).IsMouseOver;

                var targetScale = isBig
                    ? Vector2.One * CardHoverScale
                    : Vector2.One;

                model.CenterScale = model.CenterScale.MoveToward(targetScale, CardHoverGrowSpeed * delta);
            }
        }

        public CardModel GetCardModel(int cardIndex)
        {
            return _cardModels.GetChild<CardModel>(cardIndex);
        }

        public void AddCard(Card card)
        {
            AddCardModel(card, Vector2.Zero);
            RecreateCardPositioners(_cardModels.GetChildCount());
        }

        public void RemoveCard(int removedHandIndex)
        {
            RemoveCardModel(removedHandIndex);
            RecreateCardPositioners(_cardModels.GetChildCount());
        }

        public CardModel CloneCardForAnimation(int cardIndex)
        {
            var originalModel = GetCardModel(cardIndex);
            CardModel clone = ModelFactory.Create(originalModel.Card);
            AddChild(clone);
            clone.GlobalPosition = originalModel.GlobalPosition;

            return clone;
        }

        public void Refresh(Card[] cards)
        {
            // HACK: Reuse old models if nothing changed, to prevent the card
            // move animation from being needlessly interrupted.
            var oldCards = _cardModels
                .EnumerateChildren<CardModel>()
                .Select(m => m.Card);

            if (cards.SequenceEqual(oldCards))
            {
                RefreshCardModels(cards);
                return;
            }

            GD.Print($"Forcibly refreshing card row display({_forceRefreshCount++})");

            RecreateCardPositioners(cards.Length);
            RecreateCardModels(cards);
        }
        private int _forceRefreshCount = 0;

        private void RecreateCardPositioners(int cardCount)
        {
            DeleteAllChildren(_cardPositions);

            for(int i = 0; i < cardCount; i++)
            {
                var cardPositioner = new CardPositioner();
                cardPositioner.Size = CardSize;
                cardPositioner.CustomMinimumSize = CardSize;
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

        private void RecreateCardModels(Card[] cards)
        {
            DeleteAllChildren(_cardModels);

            for (int i = 0; i < cards.Length; i++)
            {
                AddCardModel(cards[i], TargetGlobalPosition(i, cards.Length));
            }
        }

        private void RefreshCardModels(Card[] cards)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                var model = _cardModels.GetChild<CardModel>(i);
                model.Refresh();
            }
        }

        private void AddCardModel(Card card, Vector2 globalPos)
        {
            var model = ModelFactory.Create(card);
            _cardModels.AddChild(model);
            model.GlobalPosition = globalPos;
        }

        private void RemoveCardModel(int index)
        {
            var holder = _cardModels.GetChild(index);
            _cardModels.RemoveChild(holder);
            holder.QueueFree();
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
                (CardSize.X + MinCardSeparation) * handCount,
                CardSize.Y
            );

            float x = (CardSize.X + MinCardSeparation) * handIndex;
            x -= totalSize.X / 2;
            x += MinCardSeparation / 2;

            var pos = _cardPositions.GlobalPosition + Vector2.Right * x;
            return pos;
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