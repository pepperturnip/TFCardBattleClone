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
        [Export] public PackedScene CardButtonPrefab;
        [Export] public CardModelFactory ModelFactory;
        [Export] public float MinCardSeparation = 8;
        [Export] public float CardMoveDecayRate = 10;
        [Export] public bool EnableInput = true;

        [Export] public int EditorPreviewCardCount = 6;

        public Vector2 CardSize {get; private set;}

        private Control _cardButtons => GetNode<Control>("%CardButtons");
        private Node2D _toLocalDummy => GetNode<Node2D>("%ToLocalDummy");

        public override void _Draw()
        {
            if (!Engine.IsEditorHint())
                return;

            for (int i = 0; i < EditorPreviewCardCount; i++)
            {
                var globalPos = TargetGlobalPosition(i, EditorPreviewCardCount);
                var localPos = _toLocalDummy.ToLocal(globalPos);

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

            for (int i = 0; i < _cardButtons.GetChildCount(); i++)
            {
                // Exponentially decay all card models to their target positions
                var model = _cardButtons.GetChild<CardButton>(i).Model;
                var targetPos = TargetGlobalPosition(i, _cardButtons.GetChildCount());

                float dist = model.GlobalPosition.DistanceTo(targetPos);
                float newDist = dist * Mathf.Pow(Mathf.E, -CardMoveDecayRate * delta);
                float deltaPos = Mathf.Abs(dist - newDist);

                model.GlobalPosition = model.GlobalPosition.MoveToward(targetPos, deltaPos);
            }
        }

        public void AddCard(Card card, Vector2 spawnPointGlobal)
        {
            var button = AddCard(card);
            button.Model.GlobalPosition = spawnPointGlobal;
        }

        private CardButton AddCard(Card card)
        {
            var button = CardButtonPrefab.Instantiate<CardButton>();
            button.SetCard(card, ModelFactory.BattleState);
            button.Pressed += () =>
            {
                if (EnableInput)
                    EmitSignal(SignalName.CardClicked, button.GetIndex());
            };

            _cardButtons.AddChild(button);
            RefreshCardLayout();

            return button;
        }

        public void RemoveCard(int removedHandIndex)
        {
            var button = _cardButtons.GetChild(removedHandIndex);
            _cardButtons.RemoveChild(button);
            button.QueueFree();

            RefreshCardLayout();
        }

        public CardButton GetCardButton(int index)
        {
            return _cardButtons.GetChild<CardButton>(index);
        }

        public CardModel CloneCardForAnimation(int cardIndex)
        {
            var originalModel = _cardButtons.GetChild<CardButton>(cardIndex).Model;
            CardModel clone = ModelFactory.Create(originalModel.Card);
            AddChild(clone);
            clone.GlobalPosition = originalModel.GlobalPosition;

            return clone;
        }

        public void Refresh(Card[] cards)
        {
            // HACK: Reuse old buttons if nothing changed, to prevent the card
            // move animation from being needlessly interrupted.
            var oldCards = _cardButtons
                .EnumerateChildren<CardButton>()
                .Select(b => b.Model.Card);

            if (cards.SequenceEqual(oldCards))
            {
                RefreshCardModels(cards);
                return;
            }

            GD.Print($"Forcibly refreshing card row display({_forceRefreshCount++})");
            RecreateCardButtons(cards);
        }
        private int _forceRefreshCount = 0;

        private void RefreshCardLayout()
        {
            int cardCount = _cardButtons.GetChildCount();

            for(int i = 0; i < cardCount; i++)
            {
                var button = _cardButtons.GetChild<CardButton>(i);

                var modelGlobalPos = button.Model.GlobalPosition;
                button.GlobalPosition = TargetGlobalPosition(i, cardCount);
                button.Model.GlobalPosition = modelGlobalPos;
            }
        }

        private void RecreateCardButtons(Card[] cards)
        {
            DeleteAllChildren(_cardButtons);

            for (int i = 0; i < cards.Length; i++)
            {
                AddCard(cards[i]);
            }
        }

        private void RefreshCardModels(Card[] cards)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                var button = _cardButtons.GetChild<CardButton>(i);
                button.SetCard(cards[i], ModelFactory.BattleState);
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
                (CardSize.X + MinCardSeparation) * handCount,
                CardSize.Y
            );

            float x = (CardSize.X + MinCardSeparation) * handIndex;
            x -= totalSize.X / 2;
            x += MinCardSeparation / 2;

            var pos = _cardButtons.GlobalPosition + Vector2.Right * x;
            return pos;
        }
    }
}