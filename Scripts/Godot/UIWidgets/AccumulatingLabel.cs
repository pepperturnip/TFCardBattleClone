using System;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class AccumulatingLabel : Control
    {
        [Export] public double AccumulationTimeSeconds = 1;

        public double DisplayedValue {get; private set;}
        public double AccumulatedDelta {get; private set;}

        private Label _displayedValueLabel => GetNode<Label>("%DisplayedValue");
        private Label _accumLabel => GetNode<Label>("%Accumulator");

        private bool _isTicking => _accumulateTimer <= 0;

        private double _accumulateTimer;

        public void AccumulateToValue(double newValue)
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
            double newDelta = newValue - DisplayedValue;
            if (ChangedDirection(AccumulatedDelta, newDelta))
            {
                SkipCurrentAnimation();
            }

            AccumulatedDelta = newValue - DisplayedValue;
            _accumulateTimer = AccumulationTimeSeconds;
        }

        /// <summary>
        /// Immediately sets the displayed value, skipping any animations
        /// </summary>
        /// <param name="newValue"></param>
        public void RefreshValue(int newValue)
        {
            DisplayedValue = newValue;
            AccumulatedDelta = 0;
            _accumulateTimer = 0;
        }

        public override void _PhysicsProcess(double delta)
        {
            if (_accumulateTimer > 0)
            {
                _accumulateTimer -= delta;
                UpdateLabels();
                return;
            }

            if (Math.Abs(AccumulatedDelta) < 1)
            {
                DisplayedValue += AccumulatedDelta;
                AccumulatedDelta = 0;
                UpdateLabels();
                return;
            }

            int sign = Math.Sign(AccumulatedDelta);
            AccumulatedDelta -= sign;
            DisplayedValue += sign;

            UpdateLabels();
        }

        private void UpdateLabels()
        {
            _displayedValueLabel.Text = DisplayedValue.ToString();

            if (AccumulatedDelta == 0)
                _accumLabel.Text = "";
            else if (AccumulatedDelta > 0)
                _accumLabel.Text = $"+{AccumulatedDelta}";
            else
                _accumLabel.Text = AccumulatedDelta.ToString();
        }

        private void SkipCurrentAnimation()
        {
            DisplayedValue += AccumulatedDelta;
            AccumulatedDelta = 0;
            _accumulateTimer = 0;
        }

        private static bool ChangedDirection(double oldDelta, double newDelta)
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