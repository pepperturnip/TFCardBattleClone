using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class MilkingMachine : ICardEffect
    {
        public async Task Activate(BattleController battle)
        {
            int draw = (int)(Math.Floor(battle.State.CustomResources["BreastSize"] / 2));

            for (int i = 0; i < draw; i++)
            {
                await battle.DrawCard();
            }
        }

        public string GetDescription(BattleState state)
            => $"Draw a number of cards equal to half of your breast size";

        public string GetOverriddenImage(BattleState state) => null;
    }
}