using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class CardCostDisplay : Node2D
    {
        public Card Card;

        private Label _brainLabel => GetNode<Label>("%BrainLabel");
        private Label _heartLabel => GetNode<Label>("%HeartLabel");
        private Label _subLabel => GetNode<Label>("%SubLabel");

        public override void _Process(double delta)
        {
            SetLabel(_brainLabel, Card?.BrainCost);
            SetLabel(_heartLabel, Card?.HeartCost);
            SetLabel(_subLabel, Card?.SubCost);

            void SetLabel(Label label, int? cost)
            {
                label.Text = "" + cost;
                label.Visible = cost != 0;
            }
        }
    }
}
