using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class CarryOverHalfDamage : ICardEffect
    {
        public Task Activate(BattleController battle)
        {
            if (!battle.State.LingeringEffects.Any(e => e is CarryOverEffect))
                battle.AddEffect(new CarryOverEffect());

            return Task.CompletedTask;
        }

        public string GetDescription(BattleState state)
            => $"1/2 TF damage carries over to next turn";

        public string GetOverriddenImage(BattleState state) => null;

        private CarryOverEffect GetOrCreateCarryOverEffect(BattleState state)
        {
            CarryOverEffect effect = state.LingeringEffects
                .Where(e => e is CarryOverEffect)
                .Cast<CarryOverEffect>()
                .FirstOrDefault();

            if (effect == null)
            {
                effect = new CarryOverEffect();
                state.LingeringEffects.Add(effect);
            }

            return effect;
        }

        private class CarryOverEffect : ILingeringEffect
        {
            private int _damage;

            public Task OnTurnEnd(BattleController battle)
            {
                _damage = battle.State.Damage;
                return Task.CompletedTask;
            }

            public Task OnResourcesDiscarded(BattleController battle)
            {
                battle.State.Damage = _damage / 2;
                battle.RemoveEffect(this);
                return Task.CompletedTask;
            }
        }
    }
}