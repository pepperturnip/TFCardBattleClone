using System;
using System.Collections.Generic;

namespace TFCardBattle.Core
{
    public class PlayerLoadout
    {
        public IEnumerable<ICard> OfferableCards;
        public IEnumerable<ICard> PermanentBuyPile;
        public IEnumerable<ICard> StartingDeck;
    }
}