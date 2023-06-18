using Godot;

namespace TFCardBattle.Godot
{
    public partial class TFBar : Control
    {
        [Export] public double Value
        {
            get => _value;
            set
            {
                _value = value;
                Refresh();
            }
        }
        private double _value;

        [Export] public double MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                Refresh();
            }
        }
        private double _maxValue;

        private ProgressBar _bar => GetNode<ProgressBar>("%Bar");
        private Label _label => GetNode<Label>("%Label");

        private void Refresh()
        {
            _bar.Value = Value;
            _bar.MaxValue = MaxValue;
            _label.Text = $"{(int)Value} / {(int)MaxValue}";
        }
    }
}