using Newtonsoft.Json;

namespace TFCardBattle.Core
{
    public readonly record struct RelicId(string Id)
    {
        public static implicit operator RelicId(string id) => new RelicId(id);
        public static implicit operator string(RelicId c) => c.Id;
        public override string ToString() => Id;
    }

    public class Relic
    {
        public string Name;
        public string IconPath;
        public string Description;

        public int TFCost;

        [JsonConverter(typeof(Parsing.LingeringEffectJsonConverter))]
        public ILingeringEffect Effect;


        public bool DebugAlwaysAdd = false;
    }
}