using System;
using System.Collections.Generic;
using System.Linq;

namespace TFCardBattle.Core
{
    public class PlayerLoadout
    {
        public IEnumerable<IEnumerable<Card>> CardPacks;
        public IEnumerable<Card> PermanentBuyPile;
        public IEnumerable<Card> StartingDeck;

        public IEnumerable<Card> OfferableCards => CardPacks.SelectMany(p => p);
    }
}