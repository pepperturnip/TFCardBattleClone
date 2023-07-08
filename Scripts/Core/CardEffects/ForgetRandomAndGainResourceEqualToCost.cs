using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class ForgetRandomAndGainResourceEqualToCost : ICardEffect
    {
        public ResourceType Resource;

        public async Task Activate(BattleController battle)
        {
            int handIndex = battle.Rng.Next(battle.State.Hand.Count);
            var card = battle.State.Hand[handIndex];
            await battle.ForgetCardInHand(handIndex);

            int cost = card.BrainCost + card.HeartCost + card.SubCost;
            int r = battle.State.GetResource(Resource);
            r += cost;
            battle.State.SetResource(Resource, r);
        }

        public string GetDescription(BattleState state)
            => $"Forget a random card in your hand.  Add {Resource} equal to its cost.";

        public string GetOverriddenImage(BattleState state) => null;
    }
}