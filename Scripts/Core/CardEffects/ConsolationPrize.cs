using System;
using System.Threading.Tasks;
using System.Linq;

namespace TFCardBattle.Core.CardEffects
{
    public class ConsolationPrize : ICardEffect
    {
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

        public string GetDescription(BattleState state)
            => $"{Resource}: +1(disposable)";

        public string GetOverriddenImage(BattleState state)
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