using System.Threading.Tasks;

namespace TFCardBattle.Core
{
    public interface ICard
    {
        string Name {get; set;}
        string Desc {get;}
        string Image {get; set;}
        string[] Gifs {get; set;}
        CardPurchaseStats PurchaseStats {get; set;}
        bool DestroyOnActivate {get;}


        Task Activate(BattleController battle);

        string GetImage(BattleState state);
    }
}