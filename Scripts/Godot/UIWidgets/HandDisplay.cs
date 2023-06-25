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

        public void PlayActivateAnimation(int cardIndex)
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

            // DEBUG: Can we play a gif at all?
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
            private static readonly Dictionary<string, double> _fileDurations =
                new Dictionary<string, double>();

            private readonly VideoStreamPlayer _player;
            private readonly string _filePath;

            private double _durationTimer = 0;

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
            }

            public override void _Process(double delta)
            {
                _durationTimer += delta;

                // Fade out as the video plays
                if (_fileDurations.ContainsKey(_filePath))
                {
                    double duration = _fileDurations[_filePath];
                    double t = _durationTimer / duration;
                    Modulate = Colors.White.Lerp(Colors.Transparent, (float)t);
                }
            }

            private void OnFinished()
            {
                // HACK: Record the duration of this video, since Godot does not
                // yet support getting the lengths of videos.
                // Fucking hell.
                if (!_fileDurations.ContainsKey(_filePath))
                    _fileDurations[_filePath] = _durationTimer;

                // Non-hack: delete the node now that it's done
                GetParent().RemoveChild(this);
                QueueFree();
            }
        }
    }
}