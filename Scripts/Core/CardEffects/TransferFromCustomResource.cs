using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class TransferFromCustomResource : ICardEffect
    {
        public CustomResourceId From;
        public ResourceType To;

        public Task Activate(BattleController battle)
        {
            int from = (int)Math.Floor(battle.State.CustomResources[From]);
            int to = battle.State.GetResource(To);
            battle.State.SetResource(To, to + from);

            return Task.CompletedTask;
        }

        public string GetDescription(BattleState state)
            => $"{state.CardRegistry.CustomResources[From].Name} => {To}";

        public string GetOverriddenImage(BattleState state) => null;
    }
}