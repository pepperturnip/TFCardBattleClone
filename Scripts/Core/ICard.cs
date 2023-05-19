namespace TFCardBattle.Core
{
    public interface ICard
    {
        string Name {get;}
        string Desc {get;}

        CardPurchaseStats PurchaseStats {get;}

        void Activate(BattleController battle);
    }
}