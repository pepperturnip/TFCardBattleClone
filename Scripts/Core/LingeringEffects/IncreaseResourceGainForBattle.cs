using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core.LingeringEffects
{
    public class IncreaseResourceGainForBattle : ILingeringEffect
    {
        public ResourceType Resource;
        public int Bonus = 1;

        private int? _before;

        Task ILingeringEffect.OnCardAboutToActivate(BattleController battle, Card card)
        {
            SetBefore(battle);
            return Task.CompletedTask;
        }

        Task ILingeringEffect.OnCardFinishedActivating(BattleController battle, Card card)
        {
            AddBonus(battle);
            return Task.CompletedTask;
        }

        Task ILingeringEffect.OnConsumableAboutToActivate(BattleController battle, Consumable c)
        {
            SetBefore(battle);
            return Task.CompletedTask;
        }

        Task ILingeringEffect.OnConsumableFinishedActivating(BattleController battle, Consumable c)
        {
            AddBonus(battle);
            return Task.CompletedTask;
        }

        private void SetBefore(BattleController battle)
        {
            _before = battle.State.GetResource(Resource);
        }

        private void AddBonus(BattleController battle)
        {
            if (!_before.HasValue)
                return;

            int after = battle.State.GetResource(Resource);

            if (after > _before.Value)
                after += Bonus;

            battle.State.SetResource(Resource, after);
            _before = null;
        }
    }
}