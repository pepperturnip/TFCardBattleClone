using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TFCardBattle.Core
{
    public static class ContentRegistry
    {
        public static IReadOnlyDictionary<CardId, Card> Cards => _cards;
        public static IReadOnlyDictionary<CardPackId, CardPack> CardPacks => _cardPacks;
        public static IReadOnlyDictionary<ConsumableId, Consumable> Consumables => _consumables;
        public static IReadOnlyDictionary<TransformationId, Transformation> Transformations => _transformations;
        public static IReadOnlyDictionary<CustomResourceId, CustomResource> CustomResources => _customResources;
        public static IReadOnlyDictionary<RelicId, Relic> Relics => _relics;


        private static Dictionary<CardId, Card> _cards = new Dictionary<CardId, Card>();
        private static Dictionary<CardPackId, CardPack> _cardPacks = new Dictionary<CardPackId, CardPack>();
        private static Dictionary<ConsumableId, Consumable> _consumables = new Dictionary<ConsumableId, Consumable>();
        private static Dictionary<TransformationId, Transformation> _transformations = new Dictionary<TransformationId, Transformation>();
        private static Dictionary<CustomResourceId, CustomResource> _customResources = new Dictionary<CustomResourceId, CustomResource>();
        private static Dictionary<RelicId, Relic> _relics = new Dictionary<RelicId, Relic>();

        public static IEnumerable<Card> CardsInPack(CardPackId id)
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
        public static void ImportCardPack(CardPackId packName, string json)
        {
            var cardPack = JsonConvert.DeserializeObject<CardPack>(json);
            _cardPacks.Add(packName, cardPack);

            foreach (var kvp in cardPack.Cards)
            {
                _cards[kvp.Key] = kvp.Value;
            }
        }

        public static void ImportConsumables(string json)
        {
            var consumables = JsonConvert.DeserializeObject<Dictionary<ConsumableId, Consumable>>(json);
            foreach (var kvp in consumables)
            {
                _consumables[kvp.Key] = kvp.Value;
            }
        }

        public static void ImportTransformation(TransformationId id, string json)
        {
            var tf = JsonConvert.DeserializeObject<Transformation>(json);
            _transformations[id] = tf;
        }

        public static void ImportCustomResources(string json)
        {
            var resources = JsonConvert.DeserializeObject<Dictionary<CustomResourceId, CustomResource>>(json);
            foreach (var kvp in resources)
            {
                _customResources[kvp.Key] = kvp.Value;
            }
        }

        public static void ImportRelic(RelicId id, string json)
        {
            var relic = JsonConvert.DeserializeObject<Relic>(json);
            _relics[id] = relic;
        }
    }
}