using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardClasses
{
    public class TransferResources : ICard
    {
        public string Name {get; set;}
        public string Desc => $"{From} => {To}";
        public string Image {get; set;}
        public string[] Gifs {get; set;}
        public bool DestroyOnActivate {get; set;}

        public ResourceType From;
        public ResourceType To;

        public CardPurchaseStats PurchaseStats {get; set;}

        public Task Activate(BattleController battle)
        {
            int from = battle.State.GetResource(From);
            int to = battle.State.GetResource(To);
            battle.State.SetResource(To, to + from);

            return Task.CompletedTask;
        }

        public string GetImage(BattleState state) => Image;
    }
}