using System;
using System.Threading.Tasks;
using System.Linq;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class PlayerHandDisplay : Control
    {
        [Signal] public delegate void CardPlayedEventHandler(int handIndex);

        [Export] public PackedScene CardModelPrefab;
        [Export] public float MinCardSeparation = 8;

        [Export] public double DrawAnimationDuration = 0.125;

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

        public async Task DrawCard(ICard[] newHand)
        {
            RefreshCardPositioners(newHand);

            // Spawn a model for the new card at the deck position.
            var newestModel = CardModelPrefab.Instantiate<CardModel>();
            newestModel.Card = newHand.Last();
            _cardModels.AddChild(newestModel);
            newestModel.GlobalPosition = Vector2.Zero;

            // Gradually move all the models to their final position
            await TweenModelsToTargetPositions(DrawAnimationDuration, newHand);

            // Refresh to ensure a consistent state
            Refresh(newHand);
        }

        public async Task RemoveCard(int removedHandIndex, ICard[] newHand)
        {
            RefreshCardPositioners(newHand);

            var modelToRemove = _cardModels.GetChild<CardModel>(removedHandIndex);
            _cardModels.RemoveChild(modelToRemove);
            modelToRemove.QueueFree();

            await TweenModelsToTargetPositions(DrawAnimationDuration, newHand);
            Refresh(newHand);
        }

        public void Refresh(ICard[] hand)
        {
            RefreshCardPositioners(hand);
            RefreshCardModels(hand);
        }

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

                cardPositioner.GlobalPosition = TargetGlobalPosition(i, hand);

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
                model.GlobalPosition = TargetGlobalPosition(i, hand);
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

        private Vector2 TargetGlobalPosition(int handIndex, ICard[] hand)
        {
            var totalSize = new Vector2(
                (_cardSize.X + MinCardSeparation) * hand.Length,
                _cardSize.Y
            );

            float x = (_cardSize.X + MinCardSeparation) * handIndex;
            x -= totalSize.X / 2;

            return _cardPositions.GlobalPosition + Vector2.Right * x;
        }

        private async Task TweenModelsToTargetPositions(double duration, ICard[] hand)
        {
            var startPositions = new Vector2[_cardModels.GetChildCount()];
            var endPositions = new Vector2[_cardModels.GetChildCount()];

            for (int i = 0; i < startPositions.Length; i++)
            {
                startPositions[i] = _cardModels.GetChild<CardModel>(i).GlobalPosition;
                endPositions[i] = TargetGlobalPosition(i, hand);
            }

            for (double timer = 0; timer < duration; timer += await WaitFor.NextFrame())
            {
                float t = (float)(timer / duration);

                for (int i = 0; i < _cardModels.GetChildCount(); i++)
                {
                    var card = _cardModels.GetChild<CardModel>(i);
                    card.GlobalPosition = startPositions[i].Lerp(endPositions[i], t);
                }
            }

            // Set all the cards to their end position, in case the timer overshot.
            for (int i = 0; i < _cardModels.GetChildCount(); i++)
            {
                var card = _cardModels.GetChild<CardModel>(i);
                card.GlobalPosition = endPositions[i];
            }
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