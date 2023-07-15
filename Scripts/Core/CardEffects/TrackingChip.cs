using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class TrackingChip : ICardEffect
    {
        public Task Activate(BattleController battle)
        {
            foreach (var resource in Enum.GetValues<ResourceType>())
                battle.AddEffect(new IncreaseResourceGainForTurn(resource, 1));

            return Task.CompletedTask;
        }

        public string GetDescription(BattleState state)
        {
            return "Whenever you gain any resource this turn, gain 1 extra.";
        }

        public string GetOverriddenImage(BattleState state) => null;
    }
}