using System.Threading.Tasks;

namespace TFCardBattle.Core.CardClasses
{
    public class JustTheTip : ICard
    {
        public string Name {get; set;}
        public string Desc => "Shield: -3\nTF: x2";
        public string Image {get; set;}
        public CardPurchaseStats PurchaseStats {get; set;}
        public bool DestroyOnActivate {get; set;}

        public Task Activate(BattleController battle)
        {
            battle.State.Shield -= 3;
            battle.State.Damage *= 2;
            return Task.CompletedTask;
        }

        public string GetImage(BattleState state) => Image;
    }
}