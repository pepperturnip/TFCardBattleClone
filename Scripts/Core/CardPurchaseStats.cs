namespace TFCardBattle.Core
{
    public struct CardPurchaseStats
    {
        public int BrainCost;
        public int HeartCost;
        public int SubCost;

        public int MinTF;
        public int MaxTF;

        /// <summary>
        /// How likely this card is to be offered to the player for purchase,
        /// if the player is within the TF range required for this card.
        /// </summary>
        public int OfferWeight;
    }
}