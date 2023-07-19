namespace TFCardBattle.Core
{
    public struct CardId
    {
        private readonly string _id;

        public CardId(string id)
        {
            _id = id;
        }

        public static implicit operator CardId(string id) => new CardId(id);
        public static implicit operator string(CardId c) => c._id;
    }

    public struct ConsumableId
    {
        private readonly string _id;

        public ConsumableId(string id)
        {
            _id = id;
        }

        public static implicit operator ConsumableId(string id) => new ConsumableId(id);
        public static implicit operator string(ConsumableId c) => c._id;
    }

    public struct CardPackId
    {
        private readonly string _id;

        public CardPackId(string id)
        {
            _id = id;
        }

        public static implicit operator CardPackId(string id) => new CardPackId(id);
        public static implicit operator string(CardPackId c) => c._id;
    }

    public struct TransformationId
    {
        private readonly string _id;

        public TransformationId(string id)
        {
            _id = id;
        }

        public static implicit operator TransformationId(string id) => new TransformationId(id);
        public static implicit operator string(TransformationId t) => t._id;
    }
}