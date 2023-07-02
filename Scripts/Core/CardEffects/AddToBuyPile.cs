using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class AddToBuyPile : ICardEffect
    {
        public string CardId;

        public async Task Activate(BattleController battle)
        {
            var state = battle.State;
            Card card = state.CardRegistry.Cards[CardId];
            state.BuyPile.Insert(state.BuyPile.Count - 1, card);

            await battle.AnimationPlayer.RefreshBuyPile(state.BuyPile.ToArray());
        }

        public string GetDescription(BattleState state)
            => $"Add the card \"{state.CardRegistry.Cards[CardId].Name}\" to the buy pile.";

        public string GetOverriddenImage(BattleState state) => null;
    }
}