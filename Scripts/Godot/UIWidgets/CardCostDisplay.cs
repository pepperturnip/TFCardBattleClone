using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class CardCostDisplay : Node2D
    {
        public ICard Card;

        private Label _brainLabel => GetNode<Label>("%BrainLabel");
        private Label _heartLabel => GetNode<Label>("%HeartLabel");
        private Label _subLabel => GetNode<Label>("%SubLabel");

        public override void _Process(double delta)
        {
            var cost = Card?.PurchaseStats;

            SetLabel(_brainLabel, cost?.BrainCost);
            SetLabel(_heartLabel, cost?.HeartCost);
            SetLabel(_subLabel, cost?.SubCost);

            void SetLabel(Label label, int? cost)
            {
                label.Text = "" + cost;
                label.Visible = cost != 0;
            }
        }
    }
}
