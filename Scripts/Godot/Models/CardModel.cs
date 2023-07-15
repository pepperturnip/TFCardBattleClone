using System;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    [Tool]
    public partial class CardModel : Control
    {
        [Export] public bool Enabled = true;
        [Export] public Color EnabledModulate = Colors.White;
        [Export] public Color DisabledModulate = Color.FromHsv(0, 0, 0.5f);
        [Export] public Vector2 CenterScale = Vector2.One;

        public Card Card => _card;
        private Card _card;

        private Panel _panel => GetNode<Panel>("%Panel");
        private TextureRect _texture => GetNode<TextureRect>("%Texture");
        private Control _fallback => GetNode<Control>("%Fallback");

        private Label _nameLabel => GetNode<Label>("%NameLabel");
        private Label _descLabel => GetNode<Label>("%DescLabel");
        private CardCostDisplay _costDisplay => GetNode<CardCostDisplay>("%CardCostDisplay");


        private BattleState _battleState;

        public override void _Ready()
        {
            if (Engine.IsEditorHint())
                return;

            Refresh();
        }

        public override void _Process(double delta)
        {
            _panel.Modulate = Enabled
                ? EnabledModulate
                : DisabledModulate;

            _panel.PivotOffset = Size / 2;
            _panel.Scale = CenterScale;
        }

        public void SetCard(Card card, BattleState state)
        {
            _card = card;
            _battleState = state;
            Refresh();
        }

        public void Refresh()
        {
            if (_card == null)
            {
                _texture.Visible = false;
                _fallback.Visible = false;
            }
            else if (ResourceLoader.Exists(Card.GetImage(_battleState)))
            {
                _texture.Visible = true;
                _fallback.Visible = false;

                _texture.Texture = ResourceLoader.Load<Texture2D>(Card.GetImage(_battleState));
            }
            else
            {
                _texture.Visible = false;
                _fallback.Visible = true;

                _nameLabel.Text = Card?.Name ?? "null";
                _descLabel.Text = Card?.GetDescription(_battleState) ?? "";
                _costDisplay.Card = Card;
            }
        }
    }
}