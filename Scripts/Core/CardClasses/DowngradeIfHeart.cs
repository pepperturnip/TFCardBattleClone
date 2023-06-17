using System;
using System.Linq;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardClasses
{
    public class DowngradeIfHeart : ICard
    {
        public string Name {get; set;}
        public string Desc => "TODO: How do I toggle this?";
        public string TexturePath {get; set;}
        public CardPurchaseStats PurchaseStats {get; set;}

        public Simple StrongVersion {get; set;}
        public Simple WeakVersion {get; set;}

        public Task Activate(BattleController battle)
        {
            return IsStrong(battle.State)
                ? StrongVersion.Activate(battle)
                : WeakVersion.Activate(battle);
        }

        public string GetTexturePath(BattleState state)
        {
            return IsStrong(state)
                ? StrongVersion.GetTexturePath(state)
                : WeakVersion.GetTexturePath(state);
        }

        private bool IsStrong(BattleState state)
        {
            return !state
                .OwnedCards
                .Any(c => c.PurchaseStats.HeartCost > 0);
        }
    }
}