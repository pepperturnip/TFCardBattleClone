using System;
using System.Threading.Tasks;
using System.Linq;

namespace TFCardBattle.Core.CardEffects
{
    public class DrawMostExpensive : ICardEffect
    {
        public ResourceType Resource {get; set;}

        private Card _consolationPrize = null;

        public Task Activate(BattleController battle)
        {
            battle.State.DrawCount++;

            Card card = battle.State.Deck
                .Where(c => c.HeartCost > 0)
                .OrderByDescending(c => c.HeartCost)
                .FirstOrDefault();

            if (card == null)
            {
                if (_consolationPrize == null)
                    _consolationPrize = CreateConsolationPrize(Resource);

                card = _consolationPrize;
            }

            battle.State.Hand.Add(card);
            battle.State.Deck.Remove(card);
            return battle.AnimationPlayer.DrawCard(card);
        }

        public string GetDescription(BattleState state)
            => $"Draw the highest-cost {Resource} card from your deck";

        public string GetOverriddenImage(BattleState state) => null;

        private Card CreateConsolationPrize(ResourceType resource)
        {
            return new Card
            {
                Name = "Consolation Prize",
                DestroyOnActivate = true,
                Effect = new ConsolationPrize(Resource)
            };
        }
    }
}