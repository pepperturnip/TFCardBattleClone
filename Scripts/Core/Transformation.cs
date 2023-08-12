namespace TFCardBattle.Core
{
    public readonly record struct TransformationId(string Id)
    {
        public static implicit operator TransformationId(string id) => new TransformationId(id);
        public static implicit operator string(TransformationId t) => t.Id;
        public override string ToString() => Id;
    }

    public class Transformation
    {
        public string Name;

        public CardPackId[] RequiredCardPacks;
    }
}