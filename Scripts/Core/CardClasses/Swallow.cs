using System.Threading.Tasks;

namespace TFCardBattle.Core.CardClasses
{
    public class Swallow : ICard
    {
        public string Name {get; set;}
        public string Desc => "Draw 3, and reset the buy piles";
        public string TexturePath {get; set;}
        public CardPurchaseStats PurchaseStats {get; set;}
        public bool DestroyOnActivate {get; set;}

        public async Task Activate(BattleController battle)
        {
            await battle.RefreshBuyPile();

            for (int i = 0; i < 3; i++)
                await battle.DrawCard();
        }

        public string GetTexturePath(BattleState state) => TexturePath;
    }
}