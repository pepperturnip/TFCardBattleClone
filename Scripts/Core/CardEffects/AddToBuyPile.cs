using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class AddToBuyPile : ICardEffect
    {
        public string CardId;

        public async Task Activate(BattleController battle)
        {
            var state = battle.State;
            Card card = ContentRegistry.Cards[CardId];
            state.BuyPile.Insert(state.BuyPile.Count - 1, card);

            await battle.AnimationPlayer.RefreshBuyPile(state.BuyPile.ToArray());
        }

        public string GetDescription(BattleState state)
            => $"Add the card \"{ContentRegistry.Cards[CardId].Name}\" to the buy pile.";

        public string GetOverriddenImage(BattleState state) => null;
    }
}