using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class TransferFromCustomResourceToRainbow : ICardEffect
    {
        public CustomResourceId Resource;

        public Task Activate(BattleController battle)
        {
            int from = (int)Math.Floor(battle.State.CustomResources[Resource]);
            battle.State.Brain += from;
            battle.State.Heart += from;
            battle.State.Sub += from;

            return Task.CompletedTask;
        }

        public string GetDescription(BattleState state)
            => $"{ContentRegistry.CustomResources[Resource].Name} => Rainbow";

        public string GetOverriddenImage(BattleState state) => null;
    }
}