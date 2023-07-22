using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TFCardBattle.Core.CardEffects
{
    public class Multi : ICardEffect
    {
        [JsonConverter(typeof(Parsing.CardEffectJsonConverter))]
        public ICardEffect[] Effects;
        public async Task Activate(BattleController battle)
        {
            foreach (var effect in Effects)
            {
                await effect.Activate(battle);
            }
        }

        public string GetDescription(BattleState state)
            => string.Join('\n', Effects.Select(e => e.GetDescription(state)));

        public string GetOverriddenImage(BattleState state) => null;
    }
}