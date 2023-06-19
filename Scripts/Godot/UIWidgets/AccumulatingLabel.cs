using System;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class AccumulatingLabel : Control
    {
        [Export] public double AccumulationTimeSeconds = 1;

        private Label _displayedValueLabel => GetNode<Label>("%DisplayedValue");
        private Label _accumLabel => GetNode<Label>("%Accumulator");

        private int _displayedValue;
        private int _accumulatedDelta;
        private double _accumulateTimer;

        public void AccumulateToValue(int newValue)
        {
            bool isTicking = _accumulateTimer <= 0;

            // Skip the ticking animation if it's already in progress
            if (isTicking)
            {
                _displayedValue += _accumulatedDelta;
                _accumulatedDelta = 0;
            }

            _accumulateTimer = AccumulationTimeSeconds;
            _accumulatedDelta = newValue - _displayedValue;
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
    }
}