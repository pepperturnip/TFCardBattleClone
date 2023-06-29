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
        }

        private void OnVideoFinished()
        {
            // Force it to loop
            _videoPlayer.Play();
        }
    }
}