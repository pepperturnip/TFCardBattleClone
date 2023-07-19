using System;
using System.Collections.Generic;
using System.Linq;

namespace TFCardBattle.Core
{
    public class PlayerLoadout
    {
        private readonly ContentRegistry _registry;

        public PlayerLoadout(ContentRegistry registry)
        {
            _registry = registry;
        }

        public IEnumerable<CardPack> ThemePacks;
        public Transformation Transformation;

        public IEnumerable<Card> PermanentBuyPile;
        public IEnumerable<Card> StartingDeck;
        public IEnumerable<Card> OfferableCards => Array.Empty<Card>()
            .Concat(Transformation.RequiredCardPacks.SelectMany(packId => _registry.CardsInPack(packId)))
            .Concat(ThemePacks.SelectMany(p => p.Cards.Values));
    }
}