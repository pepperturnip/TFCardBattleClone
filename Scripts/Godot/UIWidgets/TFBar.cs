using Godot;

namespace TFCardBattle.Godot
{
    public partial class TFBar : ProgressBar
    {
        [Export] public double TargetValue;
        [Export] public float Speed = 1;

        public override void _Process(double delta)
        {
            Value = Mathf.MoveToward(Value, TargetValue, Speed * delta);
        }
    }
}