using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TFCardBattle.Core
{
    public class ContentRegistry
    {
        public IReadOnlyDictionary<CardId, Card> Cards => _cards;
        public IReadOnlyDictionary<CardPackId, Card[]> CardPacks => _cardPacks;
        public IReadOnlyDictionary<TransformationId, Transformation> Transformations => _transformations;

        private Dictionary<CardId, Card> _cards = new Dictionary<CardId, Card>();
        private Dictionary<CardPackId, Card[]> _cardPacks = new Dictionary<CardPackId, Card[]>();
        private Dictionary<TransformationId, Transformation> _transformations= new Dictionary<TransformationId, Transformation>();

        /// <summary>
        /// Imports the given card pack into the registry.
        ///
        /// Throws an error if packName already exists.
        /// (TODO: Make it add to the existing pack)
        ///
        /// If a card pack includes a card with the same ID as an existing card,
        /// then the existing card will be overwritten (think like Skyrim mods)
        /// </summary>
        /// <param name="packName"></param>
        /// <param name="cards"></param>
        public void ImportCardPackOld(CardPackId packName, string json)
        {
            var cards = Core.Parsing.CardPacks.Parse(json);
            _cardPacks.Add(packName, cards.Values.ToArray());

            foreach (var kvp in cards)
            {
                _cards[kvp.Key] = kvp.Value;
            }
        }

        public void ImportTransformation(TransformationId id, string json)
        {
            var tf = JsonConvert.DeserializeObject<Transformation>(json);
            _transformations[id] = tf;
        }
    }
}