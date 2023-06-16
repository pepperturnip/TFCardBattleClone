using System;
using System.Collections.Generic;
using System.Linq;

namespace TFCardBattle.Core
{
    public class PlayerLoadout
    {
        public IEnumerable<IEnumerable<ICard>> CardPacks;
        public IEnumerable<ICard> PermanentBuyPile;
        public IEnumerable<ICard> StartingDeck;

        public IEnumerable<ICard> OfferableCards => CardPacks.SelectMany(p => p);
    }
}