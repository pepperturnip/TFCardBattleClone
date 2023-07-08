using System;
using System.Collections.Generic;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class HandDisplay : Control
    {
        [Signal] public delegate void CardActivatedEventHandler(int handIndex);

        [Export] public CardModelFactory ModelFactory
        {
            get => _cardRow.ModelFactory;
            set => _cardRow.ModelFactory = value;
        }

        [Export] public PackedScene GifPlayerPrefab;

        [Export] public bool EnableInput
        {
            get => _cardRow.EnableInput;
            set => _cardRow.EnableInput = value;
        }

        private CardRowDisplay _cardRow => GetNode<CardRowDisplay>("%CardRow");
        private BattleState _battleState;
        private readonly Random _rng = new Random();

        public override void _Ready()
        {
            _cardRow.CardClicked += i => EmitSignal(SignalName.CardActivated, i);
        }

        public void SetBattleState(BattleState state)
        {
            _battleState = state;
        }

        public void Refresh()
        {
            _cardRow.Refresh(_battleState.Hand.ToArray());
        }

        public async void PlayActivateAnimation(int cardIndex)
        {
            // Make a copy of the card being removed, so we can animate it
            // after removing it.
            CardModel clone = _cardRow.CloneCardForAnimation(cardIndex);
            Card card = clone.Card;

            // Detatch the clone so it doesn't shift if the "discard hand"
            // animation starts playing while the clone is still alive
            DetatchParent(clone);

            // Start animating the clone in the background.
            const double stepDuration = 0.1;
            Vector2 endPos = clone.Position + Vector2.Up * _cardRow.CardSize.Y;
            var tween = GetTree().CreateTween();

            tween.TweenProperty(
                clone,
                "position",
                endPos,
                stepDuration
            );
            tween.Parallel();
            tween.TweenProperty(
                clone,
                "CenterScale",
                Vector2.One * 1.1f,
                stepDuration
            );

            tween.TweenProperty(
                clone,
                "CenterScale",
                Vector2.One * 0.95f,
                stepDuration
            );
            tween.TweenProperty(
                clone,
                "CenterScale",
                Vector2.One,
                stepDuration
            );
            tween.TweenProperty(
                clone,
                "modulate",
                Colors.Transparent,
                stepDuration);
            tween.TweenCallback(new Callable(clone, "queue_free"));

            // Play a gif for the card
            await ToSignal(tween, Tween.SignalName.Finished);
            PlayCardGif(card, endPos);
        }

        private void PlayCardGif(Card card, Vector2 pos)
        {
            if (card.Gifs == null || card.Gifs.Length <= 0)
                return;

            var gifPlayer = GifPlayerPrefab.Instantiate<GifPlayer>();
            AddChild(gifPlayer);
            gifPlayer.Position = pos;
            gifPlayer.Play(_rng.PickFrom(card.Gifs));

            DetatchParent(gifPlayer);
        }

        private void DetatchParent(Node2D node)
        {
            var globalPos = node.GlobalPosition;

            node.GetParent().RemoveChild(node);
            GetParent().AddChild(node);

            node.GlobalPosition = globalPos;
        }

        private void DetatchParent(Control node)
        {
            var globalPos = node.GlobalPosition;

            node.GetParent().RemoveChild(node);
            GetParent().AddChild(node);

            node.GlobalPosition = globalPos;
        }

        public void AddCard(Card card)
        {
            _cardRow.AddCard(card);
        }

        public void RemoveCard(int index)
        {
            _cardRow.RemoveCard(index);
        }

        public void ClearCards()
        {
            _cardRow.Refresh(Array.Empty<Card>());
        }
    }
}