using System.Threading.Tasks;

namespace TFCardBattle.Core
{
    public class TransitioningBasicCard : ICard
    {
        public string Name => "Basic";

        public string Desc => "";

        public CardPurchaseStats PurchaseStats => default;

        public Task Activate(BattleController battle)
        {
            // HACK: Add a different resource depending on how TF'd you are
            // TODO: Don't transition _all_ basic cards at the same time!
            // TODO: Update the card's name, too.
            int tf = battle.State.PlayerTF;

            if (tf < 33)
                battle.State.Brain++;
            else if (tf < 66)
                battle.State.Heart++;
            else
                battle.State.Sub++;

            return Task.CompletedTask;
        }
    }
}