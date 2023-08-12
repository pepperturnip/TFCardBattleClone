namespace TFCardBattle.Core
{
    public readonly record struct CardId(string Id)
    {
        public static implicit operator CardId(string id) => new CardId(id);
        public static implicit operator string(CardId c) => c.Id;
        public override string ToString() => Id;
    }

    public readonly record struct ConsumableId(string Id)
    {
        public static implicit operator ConsumableId(string id) => new ConsumableId(id);
        public static implicit operator string(ConsumableId c) => c.Id;
        public override string ToString() => Id;
    }

    public readonly record struct CardPackId(string Id)
    {
        public static implicit operator CardPackId(string id) => new CardPackId(id);
        public static implicit operator string(CardPackId c) => c.Id;
        public override string ToString() => Id;
    }

    public readonly record struct TransformationId(string Id)
    {
        public static implicit operator TransformationId(string id) => new TransformationId(id);
        public static implicit operator string(TransformationId t) => t.Id;
        public override string ToString() => Id;
    }

    public readonly record struct CustomResourceId(string Id)
    {
        public static implicit operator CustomResourceId(string id) => new CustomResourceId(id);
        public static implicit operator string(CustomResourceId t) => t.Id;
        public override string ToString() => Id;
    }

}