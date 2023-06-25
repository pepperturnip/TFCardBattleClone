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

        [Export] public bool EnableInput
        {
            get => _cardRow.EnableInput;
            set => _cardRow.EnableInput = value;
        }

        private CardRowDisplay _cardRow => GetNode<CardRowDisplay>("%CardRow");
        private BattleState _battleState;

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
            CardRowDisplay.CardHolder cloneHolder = _cardRow.CloneCardForAnimation(cardIndex);

            // Start animating the clone in the background.
            const double stepDuration = 0.1;
            Vector2 endPos = cloneHolder.Position + Vector2.Up * _cardRow.CardSize.Y;
            var tween = GetTree().CreateTween();

            tween.TweenProperty(
                cloneHolder,
                "position",
                endPos,
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

            // Play a gif for the card
            // TODO: Pick from one of the gifs specified by the card data
            await ToSignal(tween,  Tween.SignalName.Finished);

            var gifPlayer = new GifPlayer("res://Media/CardGifs/17d.ogv");
            AddChild(gifPlayer);
            gifPlayer.Position = endPos;
            gifPlayer.Play();
        }

        public void AddCard(ICard card)
        {
            _cardRow.AddCard(card);
        }

        public void RemoveCard(int index)
        {
            _cardRow.RemoveCard(index);
        }

        public void ClearCards()
        {
            _cardRow.Refresh(Array.Empty<ICard>());
        }

        private partial class GifPlayer : Node2D
        {
            private const double GrowDuration = 0.1;
            private const double FadeDuration = 0.5;

            private static readonly Dictionary<string, double> _fileDurations =
                new Dictionary<string, double>();

            private readonly VideoStreamPlayer _player;
            private readonly string _filePath;

            private double _durationTimer = 0;
            private bool _durationKnown => _fileDurations.ContainsKey(_filePath);

            public GifPlayer(string filePath)
            {
                _filePath = filePath;
                _player = new VideoStreamPlayer();

                AddChild(_player);
                _player.Stream = ResourceLoader.Load<VideoStream>(filePath);
                _player.Finished += OnFinished;
            }

            public void Play()
            {
                _player.Play();

                var tween = GetTree().CreateTween();
                AddGrowAnimation(tween);
                AddFadeAnimation(tween);
            }

            public override void _Process(double delta)
            {
                _durationTimer += delta;
            }

            private void AddGrowAnimation(Tween tween)
            {
                Scale = Vector2.Zero;
                tween.TweenProperty(this, "scale", Vector2.One, GrowDuration);
            }

            private void AddFadeAnimation(Tween tween)
            {
                if (!_durationKnown)
                    return;

                double gifDuration = _fileDurations[_filePath];
                double pauseDuration = gifDuration - GrowDuration - FadeDuration;
                tween.TweenInterval(pauseDuration);
                tween.TweenProperty(this, "modulate", Colors.Transparent, FadeDuration);
            }

            private void OnFinished()
            {
                // HACK: Record the duration of this video, since Godot does not
                // yet support getting the lengths of videos.
                // Fucking hell.
                if (!_durationKnown)
                    _fileDurations[_filePath] = _durationTimer;

                // Non-hack: delete the node now that it's done
                GetParent().RemoveChild(this);
                QueueFree();
            }
        }
    }
}