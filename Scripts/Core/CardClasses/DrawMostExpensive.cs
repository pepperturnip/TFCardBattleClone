using System;
using System.Threading.Tasks;
using System.Linq;

namespace TFCardBattle.Core.CardClasses
{
    public class DrawMostExpensive : ICard
    {
        public string Name {get; set;}
        public string Desc => $"Draw the highest-cost {Resource} card from your deck";
        public string Image {get; set;}
        public string[] Gifs {get; set;}
        public CardPurchaseStats PurchaseStats {get; set;}
        public bool DestroyOnActivate {get; set;}

        public ResourceType Resource {get; set;}

        private ICard _consolationPrize = null;

        public Task Activate(BattleController battle)
        {
            battle.State.DrawCount++;

            ICard card = battle.State.Deck
                .Where(c => c.PurchaseStats.HeartCost > 0)
                .OrderByDescending(c => c.PurchaseStats.HeartCost)
                .FirstOrDefault();

            if (card == null)
            {
                if (_consolationPrize == null)
                    _consolationPrize = new ConsolationPrize(Resource);

                card = _consolationPrize;
            }

            battle.State.Hand.Add(card);
            battle.State.Deck.Remove(card);
            return battle.AnimationPlayer.DrawCard(card);
        }

        public string GetImage(BattleState state) => Image;

        private class ConsolationPrize : ICard
        {
            public string Name {get; set;} = "Consolation Prize";
            public string Desc => $"{Resource}: +1(disposable)";
            public string Image {get; set;}
            public string[] Gifs {get; set;} = Array.Empty<string>();
            public CardPurchaseStats PurchaseStats {get; set;}
            public bool DestroyOnActivate => true;

            public readonly ResourceType Resource;

            public ConsolationPrize(ResourceType resource)
            {
                Resource = resource;
            }

            public Task Activate(BattleController battle)
            {
                int value = battle.State.GetResource(Resource);
                value++;
                battle.State.SetResource(Resource, value);

                return Task.CompletedTask;
            }

            public string GetImage(BattleState state)
                => $"res://Media/Cards/{GetFileName()}";

            private string GetFileName()
            {
                switch (Resource)
                {
                    case ResourceType.Brain: return "card146.webp";
                    case ResourceType.Heart: return "card39.webp";
                    case ResourceType.Sub: return "card164.webp";

                    default: throw new InvalidOperationException(
                        $"There is no consolation prize for {Resource}"
                    );
                }
            }
        }
    }
}