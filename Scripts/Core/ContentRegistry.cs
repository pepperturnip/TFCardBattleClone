using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TFCardBattle.Core
{
    public class ContentRegistry
    {
        public IReadOnlyDictionary<CardId, Card> Cards => _cards;
        public IReadOnlyDictionary<ConsumableId, IConsumable> Consumables => _consumables;
        public IReadOnlyDictionary<CardPackId, CardPack> CardPacks => _cardPacks;
        public IReadOnlyDictionary<TransformationId, Transformation> Transformations => _transformations;

        private Dictionary<CardId, Card> _cards = new Dictionary<CardId, Card>();
        private Dictionary<CardPackId, CardPack> _cardPacks = new Dictionary<CardPackId, CardPack>();
        private Dictionary<TransformationId, Transformation> _transformations = new Dictionary<TransformationId, Transformation>();

        private Dictionary<ConsumableId, IConsumable> _consumables = new Dictionary<ConsumableId, IConsumable>
        {
            {"MinorShieldPotion", new ConsumableClasses.MinorShieldPotion()},
            {"MinorBrainPotion", new ConsumableClasses.MinorBrainPotion()},
            {"BrainPotion", new ConsumableClasses.BrainPotion()},
            {"MinorDrawPotion", new ConsumableClasses.MinorDrawPotion()}
        };

        public IEnumerable<Card> CardsInPack(CardPackId id)
        {
            return CardPacks[id].Cards.Values;
        }

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
            var cardPack = new CardPack
            {
                Name = packName,
                Cards = Parsing.CardPacks.Parse(json)
                    .ToDictionary(kvp => (CardId)kvp.Key, kvp => kvp.Value)
            };
            _cardPacks.Add(packName, cardPack);

            foreach (var kvp in cardPack.Cards)
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