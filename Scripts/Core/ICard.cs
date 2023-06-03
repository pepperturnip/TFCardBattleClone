using System.Threading.Tasks;

namespace TFCardBattle.Core
{
    public interface ICard
    {
        string Name {get;}
        string Desc {get;}

        CardPurchaseStats PurchaseStats {get;}

        Task Activate(BattleController battle);
    }
}