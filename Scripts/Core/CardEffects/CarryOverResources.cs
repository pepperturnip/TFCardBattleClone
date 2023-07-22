using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace TFCardBattle.Core.CardEffects
{
    public class CarryOverResources : ICardEffect
    {
        public ResourceType[] Resources;

        public Task Activate(BattleController battle)
        {
            var lingeringEffect = GetOrCreateCarryOverEffect(battle.State);

            foreach (var r in Resources)
            {
                lingeringEffect.ResourcesToCarry.Add(r);
            }

            return Task.CompletedTask;
        }

        public string GetDescription(BattleState state)
            => $"Unused {string.Join(',', Resources)} carries over to next turn";

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
            public HashSet<ResourceType> ResourcesToCarry
                = new HashSet<ResourceType>();

            private Dictionary<ResourceType, int> _resourceValues
                = new Dictionary<ResourceType, int>();

            public Task OnTurnEnd(BattleController battle)
            {
                foreach (var r in ResourcesToCarry)
                {
                    _resourceValues[r] = battle.State.GetResource(r);
                }
                return Task.CompletedTask;
            }

            public Task OnResourcesDiscarded(BattleController battle)
            {
                foreach (var r in ResourcesToCarry)
                {
                    battle.State.SetResource(r, _resourceValues[r]);
                }

                battle.RemoveEffect(this);
                return Task.CompletedTask;
            }
        }
    }
}