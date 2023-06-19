using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
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

        public Vector2 CardSize {get; private set;}

        private Control _cardPositions => GetNode<Control>("%CardPositions");
        private Node2D _cardHolders => GetNode<Node2D>("%CardModels");

        public override void _Ready()
        {
            // Get the size of a card by measuring a dummy one
            var dummyCard = DummyCardModelPrefab.Instantiate<CardModel>();
            CardSize = dummyCard.Size;
            dummyCard.QueueFree();
            dummyCard = null;
        }

        public override void _Process(double deltaD)
        {
            float delta = (float)deltaD;

            for (int i = 0; i < _cardHolders.GetChildCount(); i++)
            {
                // Exponentially decay all card models to their target positions
                var holder = _cardHolders.GetChild<CardHolder>(i);
                var targetPos = TargetGlobalPosition(i, _cardHolders.GetChildCount());

                float dist = holder.GlobalPosition.DistanceTo(targetPos);
                float newDist = dist * Mathf.Pow(Mathf.E, -CardMoveDecayRate * delta);
                float deltaPos = Mathf.Abs(dist - newDist);

                holder.GlobalPosition = holder.GlobalPosition.MoveToward(targetPos, deltaPos);

                // Make cards bigger when being hovered over
                var targetScale = (_cardPositions.GetChild<CardPositioner>(i).IsMouseOver && EnableInput)
                    ? Vector2.One * CardHoverScale
                    : Vector2.One;

                holder.Scaler.Scale = holder.Scaler.Scale.MoveToward(targetScale, CardHoverGrowSpeed * delta);
            }
        }

        public void AddCard(ICard card)
        {
            AddCardModel(card, Vector2.Zero);
            RecreateCardPositioners(_cardHolders.GetChildCount());
        }

        public void RemoveCard(int removedHandIndex)
        {
            RemoveCardModel(removedHandIndex);
            RecreateCardPositioners(_cardHolders.GetChildCount());
        }

        public CardHolder CloneCardForAnimation(int cardIndex)
        {
            var originalHolder = GetCardHolder(cardIndex);
            CardHolder cloneHolder = CreateCardHolder(originalHolder.Model.Card);
            AddChild(cloneHolder);
            cloneHolder.GlobalPosition = originalHolder.GlobalPosition;

            return cloneHolder;
        }

        public void PlayActivateAnimation(int cardIndex)
        {
            // Make a copy of the card being removed, so we can animate it
            // after removing it.
            CardHolder cloneHolder = CloneCardForAnimation(cardIndex);

            // Start animating the clone in the background.
            const double stepDuration = 0.1;
            var tween = GetTree().CreateTween();

            tween.TweenProperty(
                cloneHolder,
                "position",
                cloneHolder.Position + Vector2.Up * CardSize.Y,
                stepDuration
            );
            tween.Parallel();
            tween.TweenProperty(
                cloneHolder.Scaler,
                "scale",
                Vector2.One * 1.1f,
                stepDuration
            );

            tween.TweenProperty(
                cloneHolder.Scaler,
                "scale",
                Vector2.One * 0.95f,
                stepDuration
            );
            tween.TweenProperty(
                cloneHolder.Scaler,
                "scale",
                Vector2.One,
                stepDuration
            );
            tween.TweenProperty(
                cloneHolder,
                "modulate",
                Colors.Transparent,
                stepDuration);
            tween.TweenCallback(new Callable(cloneHolder, "queue_free"));
        }

        public void Refresh(ICard[] cards)
        {
            // HACK: Reuse old models if nothing changed, to prevent the card
            // move animation from being needlessly interrupted.
            var oldCards = _cardHolders
                .EnumerateChildren<CardHolder>()
                .Select(h => h.Model.Card);

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

        private void RecreateCardModels(ICard[] cards)
        {
            DeleteAllChildren(_cardHolders);

            for (int i = 0; i < cards.Length; i++)
            {
                AddCardModel(cards[i], TargetGlobalPosition(i, cards.Length));
            }
        }

        private void RefreshCardModels(ICard[] cards)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                var model = _cardHolders.GetChild<CardHolder>(i).Model;
                model.Refresh();
            }
        }

        private void AddCardModel(ICard card, Vector2 globalPos)
        {
            var model = ModelFactory.Create(card);

            var holder = new CardHolder(model);
            _cardHolders.AddChild(holder);

            holder.GlobalPosition = globalPos;
            holder.Scaler.Position = CardSize / 2;
            holder.Model.Position = -CardSize / 2;
        }

        private void RemoveCardModel(int index)
        {
            var holder = _cardHolders.GetChild(index);
            _cardHolders.RemoveChild(holder);
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

            return _cardPositions.GlobalPosition + Vector2.Right * x;
        }

        private CardHolder CreateCardHolder(ICard card)
        {
            var model = ModelFactory.Create(card);

            var holder = new CardHolder(model);
            holder.Scaler.Position = CardSize / 2;
            holder.Model.Position = -CardSize / 2;

            return holder;
        }

        private CardHolder GetCardHolder(int cardIndex)
        {
            return _cardHolders.GetChild<CardHolder>(cardIndex);
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

        public partial class CardHolder : Node2D
        {
            public Node2D Scaler {get; private set;}
            public CardModel Model {get; private set;}

            public CardHolder(CardModel model)
            {
                Model = model;
                Scaler = new Node2D();

                AddChild(Scaler);
                Scaler.AddChild(Model);
            }
        }
    }
}