using Godot;

namespace TFCardBattle.Godot
{
    public partial class AccumulatingLabel : Control
    {
        [Export] public int Value;
        [Export] public double AccumulationTimeSeconds = 1;
        [Export] public double TickingTimeSeconds = 0.1;

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
        private double _tickTimer;

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
                        _tickTimer = TickingTimeSeconds;
                        _currentState = State.Ticking;
                    }

                    break;
                }

                case State.Ticking:
                {
                    _tickTimer -= delta;
                    double t = 1.0 - (_tickTimer / TickingTimeSeconds);

                    int labelValue = (int)(Mathf.Lerp(_accumulatedValue, _displayedValue, t));

                    _accumLabel.Text = $"+{_accumulatedValue - labelValue}";
                    _displayedValueLabel.Text = labelValue.ToString();

                    if (_tickTimer <= 0)
                    {
                        _currentState = State.Idle;
                        _displayedValue = _accumulatedValue;
                        _displayedValueLabel.Text = _displayedValue.ToString();
                        _accumLabel.Text = "";
                    }

                    break;
                }
            }
        }
    }
}