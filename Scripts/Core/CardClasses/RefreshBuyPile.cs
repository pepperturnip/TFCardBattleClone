using System.Threading.Tasks;

namespace TFCardBattle.Core.CardClasses
{
    public class RefreshBuyPile : ICard
    {
        public string Name {get; set;}
        public string Desc => "Reset the buy piles";
        public string TexturePath {get; set;}
        public CardPurchaseStats PurchaseStats {get; set;}

        public Task Activate(BattleController battle)
        {
            return battle.RefreshBuyPile();
        }
    }
}