using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core.LingeringEffects
{
    public class MultiplyResourceGainForBattle : ILingeringEffect
    {
        public ResourceType Resource;
        public int Multiplier;

        private int? _before;

        Task ILingeringEffect.OnCardAboutToActivate(BattleController battle, Card card)
        {
            SetBefore(battle);
            return Task.CompletedTask;
        }

        Task ILingeringEffect.OnCardFinishedActivating(BattleController battle, Card card)
        {
            ApplyMultiplier(battle);
            return Task.CompletedTask;
        }

        Task ILingeringEffect.OnConsumableAboutToActivate(BattleController battle, Consumable c)
        {
            SetBefore(battle);
            return Task.CompletedTask;
        }

        Task ILingeringEffect.OnConsumableFinishedActivating(BattleController battle, Consumable c)
        {
            ApplyMultiplier(battle);
            return Task.CompletedTask;
        }

        private void SetBefore(BattleController battle)
        {
            _before = battle.State.GetResource(Resource);
        }

        private void ApplyMultiplier(BattleController battle)
        {
            if (!_before.HasValue)
                return;

            int after = battle.State.GetResource(Resource);
            int delta = after - _before.Value;

            if (delta > 0)
            {
                after = _before.Value + (delta * Multiplier);
            }

            battle.State.SetResource(Resource, after);
            _before = null;
        }
    }
}