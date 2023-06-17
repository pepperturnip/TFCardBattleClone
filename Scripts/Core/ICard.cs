using System.Threading.Tasks;

namespace TFCardBattle.Core
{
    public interface ICard
    {
        string Name {get; set;}
        string Desc {get;}
        string TexturePath {get; set;}

        CardPurchaseStats PurchaseStats {get; set;}

        Task Activate(BattleController battle);

        string GetTexturePath(BattleState state);
    }
}