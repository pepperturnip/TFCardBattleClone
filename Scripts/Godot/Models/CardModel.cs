using System;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    [Tool]
    public partial class CardModel : Node2D
    {
        [Export] public bool Enabled = true;
        [Export] public Color EnabledModulate = Colors.White;
        [Export] public Color DisabledModulate = Color.FromHsv(0, 0, 0.5f);

        public Vector2 Size => _panel.Size;

        public ICard Card => _card;
        private ICard _card;

        private Panel _panel => GetNode<Panel>("%Panel");
        private Label _nameLabel => GetNode<Label>("%NameLabel");
        private Label _descLabel => GetNode<Label>("%DescLabel");
        private CardCostDisplay _costDisplay => GetNode<CardCostDisplay>("%CardCostDisplay");
        private TextureRect _texture => GetNode<TextureRect>("%Texture");

        private BattleState _battleState;

        public override void _Process(double delta)
        {
            _panel.Modulate = Enabled
                ? EnabledModulate
                : DisabledModulate;
        }

        public void SetCard(ICard card, BattleState state)
        {
            _card = card;
            _battleState = state;
            Refresh();
        }

        public void Refresh()
        {
            _nameLabel.Text = Card?.Name ?? "null";
            _descLabel.Text = Card?.Desc ?? "";
            _costDisplay.Card = Card;

            // Attempt to load the texture.  If it exists, we'll draw that on
            // top of the placeholder stuff.  If it doesn't, we'll hide the
            // texture and use the placeholder stuff.
            if (!ResourceLoader.Exists(Card.GetImage(_battleState)))
            {
                _texture.Visible = false;
                GD.Print($"Could not find texture {Card.GetImage(_battleState)}");
                return;
            }

            _texture.Texture = ResourceLoader.Load<Texture2D>(Card.GetImage(_battleState));
            _texture.Visible = true;
        }
    }
}