using System;

namespace TFCardBattle.Core
{
    /// <summary>
    /// A card that merely grants resources, without any other special effects.
    /// </summary>
    public class SimpleCard : ICard
    {
        public string Name {get; set;}
        public string Desc => GetDescription();

        public int BrainGain {get; set;}
        public int HeartGain {get; set;}
        public int SubGain {get; set;}
        public int ShieldGain {get; set;}
        public int TFGain {get; set;}
        public int CardDraw {get; set;}

        public CardPurchaseStats PurchaseStats {get; set;}

        private string _description;
        private int _descriptionHash;

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

        private string GetDescription()
        {
            // Since this could get called every frame, don't re-generate the
            // description unless something has changed.  This way we avoid
            // creating extra work for the garbage collector.
            int newHash = GetDescriptionHash();

            if (newHash == _descriptionHash)
                return _description;

            // OK, the description doesn't match the resources.  Re-generate it.
            var builder = new System.Text.StringBuilder();

            LabelFor("Brain", BrainGain);
            LabelFor("Heart", HeartGain);
            LabelFor("Sub", SubGain);
            LabelFor("Shield", ShieldGain);
            LabelFor("TF", TFGain);
            LabelFor("Draw", CardDraw);

            _description = builder.ToString();
            _descriptionHash = newHash;
            return _description;

            int GetDescriptionHash()
            {
                var hash = new HashCode();
                hash.Add(BrainGain);
                hash.Add(HeartGain);
                hash.Add(SubGain);
                hash.Add(ShieldGain);
                hash.Add(TFGain);
                hash.Add(CardDraw);

                return hash.ToHashCode();
            }

            void LabelFor(string resource, int value)
            {
                if (value == 0)
                    return;
                else if (value > 0)
                    builder.AppendLine($"{resource}: +{value}");
                else
                    builder.AppendLine($"{resource}: {value}");
            }
        }
    }
}