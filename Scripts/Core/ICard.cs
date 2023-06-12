using System.Threading.Tasks;

namespace TFCardBattle.Core
{
    public interface ICard
    {
        string Name {get;}
        string Desc {get;}
        string TexturePath {get;}

        CardPurchaseStats PurchaseStats {get;}

        Task Activate(BattleController battle);
    }
}