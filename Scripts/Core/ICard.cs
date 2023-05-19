namespace TFCardBattle.Core
{
    public interface ICard
    {
        string Name {get;}
        string Desc {get;}

        int BrainCost {get;}
        int HeartCost {get;}
        int SubsCost {get;}

        int MinTF {get;}
        int MaxTF {get;}

        /// <summary>
        /// How likely this card is to be offered to the player for purchase,
        /// if the player is within the TF range required for this card.
        /// </summary>
        /// <value></value>
        int OfferWeight {get;}

        void Activate(BattleState battleState);
    }
}