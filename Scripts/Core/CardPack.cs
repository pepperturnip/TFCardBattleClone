using System.Collections.Generic;

namespace TFCardBattle.Core
{
    public class CardPack
    {
        public string Name;
        public CardPackType Type = CardPackType.Standard;
        public IReadOnlyDictionary<CardId, Card> Cards;
    }

    public enum CardPackType
    {
        Standard,
        Core,
        Transformation,
        PermanentBuyPile
    }
}