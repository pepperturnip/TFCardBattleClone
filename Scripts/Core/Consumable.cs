using Newtonsoft.Json;

namespace TFCardBattle.Core
{
    public readonly record struct ConsumableId(string Id)
    {
        public static implicit operator ConsumableId(string id) => new ConsumableId(id);
        public static implicit operator string(ConsumableId c) => c.Id;
        public override string ToString() => Id;
    }

    public class Consumable
    {
        public string Name;
        public string IconPath;

        [JsonConverter(typeof(Parsing.CardEffectJsonConverter))]
        public ICardEffect Effect;
    }
}