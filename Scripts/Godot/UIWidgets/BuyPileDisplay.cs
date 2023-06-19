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
        private BattleState _battleState;

        public override void _Ready()
        {
            _cardRow.CardClicked += i => EmitSignal(SignalName.CardBought, i);
        }

        public void SetBattleState(BattleState state)
        {
            _battleState = state;
        }

        public void Refresh()
        {
            _cardRow.Refresh(_battleState.BuyPile.ToArray());

            // Disable cards that the player can't afford
            for (int i = 0; i < _battleState.BuyPile.Count; i++)
            {
                _cardRow.GetCardModel(i).Enabled = _battleState.CanAffordCard(i);
            }
        }

        public void PlayBuyAnimation(int cardIndex)
        {
            CardRowDisplay.CardHolder cloneHolder = _cardRow.CloneCardForAnimation(cardIndex);

            // Start animating the clone in the background.
            const double stepDuration = 0.1;
            var tween = GetTree().CreateTween();

            tween.TweenProperty(
                cloneHolder,
                "position",
                cloneHolder.Position + Vector2.Down * _cardRow.CardSize,
                stepDuration
            );
            tween.Parallel();
            tween.TweenProperty(
                cloneHolder.Scaler,
                "scale",
                Vector2.Zero,
                stepDuration
            );
            tween.TweenCallback(new Callable(cloneHolder, "queue_free"));
        }

        public void RemoveCard(int index)
        {
            _cardRow.RemoveCard(index);
        }
    }
}