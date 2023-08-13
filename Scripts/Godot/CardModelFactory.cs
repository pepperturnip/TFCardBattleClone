using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class CardModelFactory : Node
    {
        [Export] public PackedScene CardModelPrefab;

        public BattleState BattleState;

        public void SetBattleState(BattleState state)
        {
            BattleState = state;
        }

        public CardModel Create(Card card)
        {
            var model = CardModelPrefab.Instantiate<CardModel>();
            model.SetCard(card, BattleState);

            return model;
        }
    }
}