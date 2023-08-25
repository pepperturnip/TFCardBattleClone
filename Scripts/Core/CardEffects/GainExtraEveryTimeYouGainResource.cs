using System.Threading.Tasks;
using TFCardBattle.Core.LingeringEffects;

namespace TFCardBattle.Core.CardEffects
{
    public class GainExtraEveryTimeYouGainResource : ICardEffect
    {
        public ResourceType Resource;
        public int ImmediateGain;
        public int BonusAmount;

        public Task Activate(BattleController battle)
        {
            int resourceValue = battle.State.GetResource(Resource);
            resourceValue += ImmediateGain;
            battle.State.SetResource(Resource, resourceValue);

            battle.AddEffect(new IncreaseResourceGainForTurn(Resource, BonusAmount));
            return Task.CompletedTask;
        }

        public string GetDescription(BattleState state)
        {
            return
                $"{Resource}: +{ImmediateGain}\n" +
                $"Every time you gain {Resource} this turn, gain {BonusAmount} more.";
        }

        public string GetOverriddenImage(BattleState state) => null;
    }
}