using System;
using System.Linq;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class StartMaid : ICardEffect, ILingeringEffect
    {
        public Task Activate(BattleController battle)
        {
            if (!battle.State.LingeringEffects.Any(e => e is StartMaid))
                battle.AddEffect(this);

            return Task.CompletedTask;
        }

        public string GetDescription(BattleState state)
            => "Start increasing the maid counter";

        public string GetOverriddenImage(BattleState state) => null;

        Task ILingeringEffect.OnTurnEnd(BattleController battle)
        {
            battle.State.CustomResources["MaidDirty"]++;
            return Task.CompletedTask;
        }
    }
}