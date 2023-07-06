using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class TransferRainbowToDamage : ICardEffect
    {
        public Task Activate(BattleController battle)
        {
            battle.State.Damage += battle.State.Brain;
            battle.State.Damage += battle.State.Heart;
            battle.State.Damage += battle.State.Sub;

            return Task.CompletedTask;
        }

        public string GetDescription(BattleState state) => $"Brain/Heart/Sub => Damage";

        public string GetOverriddenImage(BattleState state) => null;
    }
}