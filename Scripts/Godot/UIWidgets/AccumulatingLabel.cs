using System;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class AccumulatingLabel : Control
    {
        [Export] public double AccumulationTimeSeconds = 1;

        private Label _displayedValueLabel => GetNode<Label>("%DisplayedValue");
        private Label _accumLabel => GetNode<Label>("%Accumulator");

        private bool _isTicking => _accumulateTimer <= 0;

        private int _displayedValue;
        private int _accumulatedDelta;
        private double _accumulateTimer;

        public void AccumulateToValue(int newValue)
        {
            // Skip the animation if we're already ticking
            if (_isTicking)
            {
                SkipCurrentAnimation();
            }

            // Skip the animation if we've changed directions while accumulating.
            // That way, the player won't see the accumulator go from "+5" to
            // "+4" if they spend a resource really fast; instead, it'll display
            // "-1" as you'd expect.
            int newDelta = newValue - _displayedValue;
            if (ChangedDirection(_accumulatedDelta, newDelta))
            {
                SkipCurrentAnimation();
            }

            _accumulatedDelta = newValue - _displayedValue;
            _accumulateTimer = AccumulationTimeSeconds;
        }

        /// <summary>
        /// Immediately sets the displayed value, skipping any animations
        /// </summary>
        /// <param name="newValue"></param>
        public void RefreshValue(int newValue)
        {
            _displayedValue = newValue;
            _accumulatedDelta = 0;
            _accumulateTimer = 0;
        }

        public override void _PhysicsProcess(double delta)
        {
            if (_accumulateTimer > 0)
            {
                _accumulateTimer -= delta;
            }
            else
            {
                int sign = Math.Sign(_accumulatedDelta);
                _accumulatedDelta -= sign;
                _displayedValue += sign;
            }

            UpdateLabels();
        }

        private void UpdateLabels()
        {
            _displayedValueLabel.Text = _displayedValue.ToString();

            if (_accumulatedDelta == 0)
                _accumLabel.Text = "";
            else if (_accumulatedDelta > 0)
                _accumLabel.Text = $"+{_accumulatedDelta}";
            else
                _accumLabel.Text = _accumulatedDelta.ToString();
        }

        private void SkipCurrentAnimation()
        {
            _displayedValue += _accumulatedDelta;
            _accumulatedDelta = 0;
            _accumulateTimer = 0;
        }

        private static bool ChangedDirection(int oldDelta, int newDelta)
        {
            if (oldDelta > 0)
                return newDelta < oldDelta;
            else if (oldDelta < 0)
                return newDelta > oldDelta;
            else // if (oldDelta == 0)
                return newDelta != oldDelta;
        }
    }
}