namespace TFCardBattle.Core
{
    /// <summary>
    /// A card that merely grants resources, without any other special effects.
    /// </summary>
    public class SimpleCard : ICard
    {
        public string Name {get; set;}
        public string Desc {get; set;}

        public int BrainGain {get; set;}
        public int HeartGain {get; set;}
        public int SubsGain {get; set;}
        public int ShieldGain {get; set;}
        public int TFGain {get; set;}

        public CardPurchaseStats PurchaseStats {get; set;}

        public void Activate(BattleState battleState)
        {
            battleState.Brain += BrainGain;
            battleState.Heart += HeartGain;
            battleState.Subs += SubsGain;
            battleState.Shield += ShieldGain;
            battleState.TF += TFGain;
        }
    }
}