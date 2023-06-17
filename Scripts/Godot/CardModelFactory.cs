using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class CardModelFactory : Node
    {
        [Export] public PackedScene CardModelPrefab;

        private BattleState _battleState;

        public void SetBattleState(BattleState state)
        {
            _battleState = state;
        }

        public CardModel Create(ICard card)
        {
            var model = CardModelPrefab.Instantiate<CardModel>();
            model.SetCard(card, _battleState);

            return model;
        }
    }
}