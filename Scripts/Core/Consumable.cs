using Newtonsoft.Json;

namespace TFCardBattle.Core
{
    public class Consumable
    {
        public string Name;

        [JsonConverter(typeof(Parsing.CardEffectJsonConverter))]
        public ICardEffect Effect;
    }
}