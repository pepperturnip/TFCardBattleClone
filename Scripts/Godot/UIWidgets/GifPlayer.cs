using System;
using System.Collections.Generic;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class GifPlayer : Node2D
    {
        private const double TotalDuration = 2;
        private const double GrowDuration = 0.1;
        private const double FadeDuration = 0.5;

        [Export] public Vector2 Size = new Vector2(320, 160);

        private VideoStreamPlayer _videoPlayer => GetNode<VideoStreamPlayer>("%VideoPlayer");

        public void Play(string filePath)
        {
            _videoPlayer.Stream = ResourceLoader.Load<VideoStream>(filePath);
            _videoPlayer.Play();

            var tween = GetTree().CreateTween();
            _videoPlayer.Scale = Vector2.Zero;
            tween.TweenProperty(_videoPlayer, "scale", Vector2.One, GrowDuration);
            tween.TweenInterval(TotalDuration - GrowDuration - FadeDuration);
            tween.TweenProperty(this, "modulate", Colors.Transparent, FadeDuration);
            tween.TweenCallback(new Callable(this, "queue_free"));
        }

        public override void _Process(double delta)
        {
            _videoPlayer.PivotOffset = _videoPlayer.Size / 2;

            // Stretch the video such that it fits within Size, while keeping
            // its original aspect ratio.
            float s = Size.X < _videoPlayer.Size.Y
                ? Size.Y / _videoPlayer.Size.Y
                : Size.X / _videoPlayer.Size.X;

            Scale = s * Vector2.One;
        }

        private void OnVideoFinished()
        {
            // Force it to loop
            _videoPlayer.Play();
        }
    }
}