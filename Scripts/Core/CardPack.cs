using System.Collections.Generic;

namespace TFCardBattle.Core
{
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