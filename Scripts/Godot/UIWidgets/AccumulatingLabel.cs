using Godot;

namespace TFCardBattle.Godot
{
    public partial class AccumulatingLabel : Control
    {
        [Export] public int Value;
        [Export] public double AccumulationTimeSeconds = 1;
        [Export] public double TickingTimeSeconds = 1;

        private enum State
        {
            Idle,
            Accumulating,
            Ticking
        }
        private State _currentState = State.Idle;

        private int _displayedValue;
        private int _accumulatedValue;

        private double _accumulateTimer;

        private Label _displayedValueLabel => GetNode<Label>("%DisplayedValue");
        private Label _accumLabel => GetNode<Label>("%Accumulator");

        public override void _Process(double delta)
        {
            switch (_currentState)
            {
                case State.Idle:
                {
                    // Start accumulating if the value has changed
                    if (Value != _displayedValue)
                    {
                        _currentState = State.Accumulating;
                        _accumulateTimer = AccumulationTimeSeconds;
                        _accumulatedValue = Value;
                    }

                    break;
                }

                case State.Accumulating:
                {
                    _accumLabel.Text = $"+{Value - _displayedValue}";

                    // Reset the timer if the value changed again while we're
                    // accumulating
                    if (Value != _accumulatedValue)
                    {
                        _accumulatedValue = Value;
                        _accumulateTimer = AccumulationTimeSeconds;
                    }

                    // Start moving the displayed value toward the real value
                    // when the timer runs out
                    _accumulateTimer -= delta;
                    if (_accumulateTimer <= 0)
                    {
                        // TODO: Start ticking instead of going back to idle
                        _accumLabel.Text = "";
                        _displayedValueLabel.Text = Value.ToString();
                        _currentState = State.Idle;
                    }

                    break;
                }
            }
        }
    }
}