using System;
using System.Collections.Generic;
using System.Linq;

namespace TFCardBattle.Core
{
    public class PlayerLoadout
    {
        public IEnumerable<IEnumerable<Card>> ThemePacks;
        public Transformation Transformation;

        public IEnumerable<Card> PermanentBuyPile => Transformation.PermanentBuyPile;
        public IEnumerable<Card> StartingDeck => Transformation.StartingDeck;
        public IEnumerable<Card> OfferableCards => Array.Empty<Card>()
            .Concat(Transformation.CardPack)
            .Concat(ThemePacks.SelectMany(p => p));
    }
}