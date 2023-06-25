using System;
using System.Collections.Generic;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class GifPlayer : Node2D
    {
        private const double GrowDuration = 0.1;
        private const double FadeDuration = 0.5;

        private static readonly Dictionary<string, double> _fileDurations =
            new Dictionary<string, double>();

        private VideoStreamPlayer _videoPlayer => GetNode<VideoStreamPlayer>("%VideoPlayer");
        private string _filePath;

        private double _durationTimer = 0;
        private bool _durationKnown => _fileDurations.ContainsKey(_filePath);

        public void Play(string filePath)
        {
            _filePath = filePath;
            _videoPlayer.Stream = ResourceLoader.Load<VideoStream>(filePath);
            _videoPlayer.Play();

            var tween = GetTree().CreateTween();
            AddGrowAnimation(tween);
            AddFadeAnimation(tween);

            _durationTimer = 0;
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

        private void OnVideoFinished()
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