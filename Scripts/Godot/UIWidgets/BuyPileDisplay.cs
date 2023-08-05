using System;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class BuyPileDisplay : Control
    {
        [Signal] public delegate void CardBoughtEventHandler(int buyPileIndex);

        [Export] public CardModelFactory ModelFactory
        {
            get => _cardRow.ModelFactory;
            set => _cardRow.ModelFactory = value;
        }

        [Export] public bool EnableInput
        {
            get => _cardRow.EnableInput;
            set => _cardRow.EnableInput = value;
        }

        private CardRowDisplay _cardRow => GetNode<CardRowDisplay>("%CardRow");
        private BattleController _battle;

        public override void _Ready()
        {
            _cardRow.CardClicked += i => EmitSignal(SignalName.CardBought, i);
        }

        public void SetBattle(BattleController battle)
        {
            _battle = battle;
        }

        public void Refresh()
        {
            _cardRow.Refresh(_battle.State.BuyPile.ToArray());

            // Disable cards that the player can't afford
            for (int i = 0; i < _battle.State.BuyPile.Count; i++)
            {
                _cardRow.GetCardModel(i).Enabled = _battle.CanAffordCard(i);
            }
        }

        public void PlayBuyAnimation(int cardIndex)
        {
            CardModel clone = _cardRow.CloneCardForAnimation(cardIndex);

            // Start animating the clone in the background.
            const double stepDuration = 0.1;
            var tween = GetTree().CreateTween();

            tween.TweenProperty(
                clone,
                "position",
                clone.Position + Vector2.Down * _cardRow.CardSize,
                stepDuration
            );
            tween.Parallel();
            tween.TweenProperty(
                clone,
                "CenterScale",
                Vector2.Zero,
                stepDuration
            );
            tween.TweenCallback(new Callable(clone, "queue_free"));
        }

        public void RemoveCard(int index)
        {
            _cardRow.RemoveCard(index);
        }
    }
}