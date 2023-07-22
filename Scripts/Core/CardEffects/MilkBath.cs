using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class MilkBath : ICardEffect
    {
        public Task Activate(BattleController battle)
        {
            int healing = (int)(Math.Floor(battle.State.CustomResources["BreastSize"] / 3));
            battle.State.PlayerTF -= healing;

            return Task.CompletedTask;
        }

        public string GetDescription(BattleState state)
            => $"Heal TF equal to one third of your breast size";

        public string GetOverriddenImage(BattleState state) => null;
    }
}