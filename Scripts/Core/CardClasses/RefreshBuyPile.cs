using System.Threading.Tasks;

namespace TFCardBattle.Core.CardClasses
{
    public class RefreshBuyPile : ICard
    {
        public string Name {get; set;}
        public string Desc => "Reset the buy piles";
        public string Image {get; set;}
        public CardPurchaseStats PurchaseStats {get; set;}
        public bool DestroyOnActivate {get; set;}

        public Task Activate(BattleController battle)
        {
            return battle.RefreshBuyPile();
        }

        public string GetImage(BattleState state) => Image;
    }
}