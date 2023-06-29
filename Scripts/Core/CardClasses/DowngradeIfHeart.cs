using System;
using System.Linq;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardClasses
{
    public class DowngradeIfHeart : ICard
    {
        public string Name {get; set;}
        public string Desc => "TODO: How do I toggle this?";
        public string Image {get; set;}
        public string[] Gifs {get; set;}
        public CardPurchaseStats PurchaseStats {get; set;}
        public bool DestroyOnActivate {get; set;}

        public Simple StrongVersion {get; set;}
        public Simple WeakVersion {get; set;}

        public Task Activate(BattleController battle)
        {
            return IsStrong(battle.State)
                ? StrongVersion.Activate(battle)
                : WeakVersion.Activate(battle);
        }

        public string GetImage(BattleState state)
        {
            return IsStrong(state)
                ? StrongVersion.GetImage(state)
                : WeakVersion.GetImage(state);
        }

        private bool IsStrong(BattleState state)
        {
            return !state
                .OwnedCards
                .Any(c => c.PurchaseStats.HeartCost > 0);
        }
    }
}