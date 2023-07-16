using System.Collections.Generic;

namespace TFCardBattle.Core
{
    public class CardPack
    {
        public string Name;
        public IReadOnlyDictionary<CardId, Card> Cards;
    }
}