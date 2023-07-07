using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class CopyLastCardPlayed : ICardEffect
    {
        public ResourceType Resource;

        public Task Activate(BattleController battle)
        {
            throw new NotImplementedException();
        }

        public string GetDescription(BattleState state)
            => "Copy the last card played this turn";

        public string GetOverriddenImage(BattleState state) => null;
    }
}