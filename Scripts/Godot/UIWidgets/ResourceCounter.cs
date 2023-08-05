using Godot;

namespace TFCardBattle.Godot
{
    public partial class ResourceCounter : TextureRect
    {
        private AccumulatingLabel _valueLabel => GetNode<AccumulatingLabel>("%ValueLabel");

        public override void _Process(double delta)
        {
            Visible =
                _valueLabel.DisplayedValue != 0 ||
                _valueLabel.AccumulatedDelta != 0;
        }

        public void AccumulateToValue(double newValue)
            => _valueLabel.AccumulateToValue(newValue);

        public void RefreshValue(double newValue)
            => _valueLabel.RefreshValue(newValue);
    }
}