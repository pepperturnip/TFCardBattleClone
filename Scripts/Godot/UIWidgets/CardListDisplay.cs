using System.Collections.Generic;
using TFCardBattle.Core;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class CardListDisplay : Control
    {
        [Export] public CardModelFactory ModelFactory;
        [Export] public Vector2 CardSize = new Vector2(150, 210);

        private Control _cardContainer => GetNode<Control>("%CardContainer");

        public void Refresh(IEnumerable<Card> cards)
        {
            while (_cardContainer.GetChildCount() > 0)
            {
                var child = _cardContainer.GetChild(0);
                _cardContainer.RemoveChild(child);
                child.QueueFree();
            }

            foreach (Card card in cards)
            {
                var model = ModelFactory.Create(card);
                model.CustomMinimumSize = CardSize;

                _cardContainer.AddChild(model);
            }
        }
    }
}