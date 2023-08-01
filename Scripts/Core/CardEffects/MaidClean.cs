using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class MaidClean : ICardEffect
    {
        public int Amount = 1;

        public Task Activate(BattleController battle)
        {
            battle.State.CustomResources["MaidDirty"] -= Amount;

            if (battle.State.CustomResources["MaidDirty"] < 0)
                battle.State.CustomResources["MaidDirty"] = 0;

            return Task.CompletedTask;
        }

        public string GetDescription(BattleState state) => Amount < 0
            ? $"Dirtiness: {Amount}"
            : $"Dirtiness: +{Amount}";

        public string GetOverriddenImage(BattleState state) => null;
    }
}