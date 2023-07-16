using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core
{
    public class IncreaseResourceGainForTurn : ILingeringEffect
    {
        private int? _before;

        private readonly ResourceType _resource;
        private readonly int _bonus;

        public IncreaseResourceGainForTurn(ResourceType resource, int bonus)
        {
            _resource = resource;
            _bonus = bonus;
        }

        Task ILingeringEffect.OnTurnEnd(BattleController battle)
        {
            battle.RemoveEffect(this);
            return Task.CompletedTask;
        }

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

        Task ILingeringEffect.OnConsumableAboutToActivate(BattleController battle, IConsumable c)
        {
            SetBefore(battle);
            return Task.CompletedTask;
        }

        Task ILingeringEffect.OnConsumableFinishedActivating(BattleController battle, IConsumable c)
        {
            AddBonus(battle);
            return Task.CompletedTask;
        }

        private void SetBefore(BattleController battle)
        {
            _before = battle.State.GetResource(_resource);
        }

        private void AddBonus(BattleController battle)
        {
            if (!_before.HasValue)
                return;

            int after = battle.State.GetResource(_resource);

            if (after > _before.Value)
                after += _bonus;

            battle.State.SetResource(_resource, after);
            _before = null;
        }
    }
}