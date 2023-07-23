using Newtonsoft.Json;

namespace TFCardBattle.Core
{
    public class Consumable
    {
        public string Name;
        public string IconPath;

        [JsonConverter(typeof(Parsing.CardEffectJsonConverter))]
        public ICardEffect Effect;
    }
}