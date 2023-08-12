using System.Collections.Generic;

namespace TFCardBattle.Core
{
    public readonly record struct CardPackId(string Id)
    {
        public static implicit operator CardPackId(string id) => new CardPackId(id);
        public static implicit operator string(CardPackId c) => c.Id;
        public override string ToString() => Id;
    }

    public class CardPack
    {
        public string Name;
        public CardPackType Type = CardPackType.Core;
        public IReadOnlyDictionary<CardId, Card> Cards;

        public bool CanBeEquipped()
        {
            return
                (Type == CardPackType.BrainSlot) ||
                (Type == CardPackType.HeartSlot) ||
                (Type == CardPackType.SubSlot);
        }
    }

    public enum CardPackType
    {
        Core,
        Transformation,
        PermanentBuyPile,
        BrainSlot,
        HeartSlot,
        SubSlot
    }
}