using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class TransferResources : ICardEffect
    {
        public ResourceType From;
        public ResourceType To;

        public Task Activate(BattleController battle)
        {
            int from = battle.State.GetResource(From);
            int to = battle.State.GetResource(To);
            battle.State.SetResource(To, to + from);

            return Task.CompletedTask;
        }

        public string GetDescription(BattleState state) => $"{From} => {To}";

        public string GetOverriddenImage(BattleState state) => null;
    }
}