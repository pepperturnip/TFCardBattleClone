using System.Threading.Tasks;

namespace TFCardBattle.Core.CardClasses
{
    public class MentalMastery : ICard
    {
        public string Name {get; set;}
        public string Desc => "Remove a basic card from your deck.  Draw 1.";
        public string Image {get; set;}
        public CardPurchaseStats PurchaseStats {get; set;}
        public bool DestroyOnActivate {get; set;}

        public async Task Activate(BattleController battle)
        {
            await battle.ForgetBasicCard();
            await battle.DrawCard();
        }

        public string GetImage(BattleState state) => Image;
    }
}