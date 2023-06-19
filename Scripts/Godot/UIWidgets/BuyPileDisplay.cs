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
        }

        public void PlayBuyAnimation(int buyPileIndex)
        {
            _cardRow.PlayBuyAnimation(buyPileIndex);
        }

        public void RemoveCard(int index)
        {
            _cardRow.RemoveCard(index);
        }
    }
}