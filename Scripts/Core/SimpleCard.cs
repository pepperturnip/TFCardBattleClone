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
        public int SubGain {get; set;}
        public int ShieldGain {get; set;}
        public int TFGain {get; set;}
        public int CardDraw {get; set;}

        public CardPurchaseStats PurchaseStats {get; set;}

        public void Activate(BattleController battle)
        {
            battle.State.Brain += BrainGain;
            battle.State.Heart += HeartGain;
            battle.State.Sub += SubGain;
            battle.State.Shield += ShieldGain;
            battle.State.TF += TFGain;

            for (int i = 0; i < CardDraw; i++)
            {
                battle.DrawCard();
            }
        }
    }
}