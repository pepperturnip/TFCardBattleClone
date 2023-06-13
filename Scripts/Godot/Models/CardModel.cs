using System;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class CardModel : Node2D
    {
        public Vector2 Size => _panel.Size;

        public ICard Card
        {
            get => _card;
            set
            {
                _card = value;
                Refresh();
            }
        }
        private ICard _card;

        private Panel _panel => GetNode<Panel>("%Panel");
        private Label _nameLabel => GetNode<Label>("%NameLabel");
        private Label _descLabel => GetNode<Label>("%DescLabel");
        private CardCostDisplay _costDisplay => GetNode<CardCostDisplay>("%CardCostDisplay");
        private TextureRect _texture => GetNode<TextureRect>("%Texture");

        public void Refresh()
        {
            _nameLabel.Text = Card?.Name ?? "null";
            _descLabel.Text = Card?.Desc ?? "";
            _costDisplay.Card = Card;

            // Attempt to load the texture.  If it exists, we'll draw that on
            // top of the placeholder stuff.  If it doesn't, we'll hide the
            // texture and use the placeholder stuff.
            if (!ResourceLoader.Exists(Card.TexturePath))
            {
                _texture.Visible = false;
                GD.Print($"Could not find texture {Card.TexturePath}");
                return;
            }

            _texture.Texture = ResourceLoader.Load<Texture2D>(Card.TexturePath);
            _texture.Visible = true;
        }
    }
}