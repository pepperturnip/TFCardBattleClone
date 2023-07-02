using System;
using System.Linq;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    public class DowngradeIfResource : ICardEffect
    {
        public ResourceType Resource {get; set;}
        public VersionedSimple StrongVersion {get; set;}
        public VersionedSimple WeakVersion {get; set;}

        public Task Activate(BattleController battle)
        {
            return IsStrong(battle.State)
                ? StrongVersion.Activate(battle)
                : WeakVersion.Activate(battle);
        }

        public string GetDescription(BattleState state)
        {
            return IsStrong(state)
                ? StrongVersion.GetDescription(state)
                : WeakVersion.GetDescription(state);
        }

        public string GetOverriddenImage(BattleState state)
        {
            return IsStrong(state)
                ? StrongVersion.GetOverriddenImage(state)
                : WeakVersion.GetOverriddenImage(state);
        }

        private bool IsStrong(BattleState state)
        {
            return !state
                .OwnedCards
                .Any(c => c.GetCost(Resource) > 0);
        }

        public class VersionedSimple : Simple
        {
            public string Image;
            public override string GetOverriddenImage(BattleState state) => Image;
        }
    }
}