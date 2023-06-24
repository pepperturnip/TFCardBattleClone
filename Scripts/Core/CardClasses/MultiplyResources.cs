using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardClasses
{
    /// <summary>
    /// A card that multiplies the player's current resources by some number.
    /// </summary>
    public class MultiplyResources : ICard
    {
        public string Name {get; set;}
        public string Desc => GetDescription();
        public string Image {get; set;}
        public CardPurchaseStats PurchaseStats {get; set;}
        public bool DestroyOnActivate {get; set;}

        public int BrainMult {get; set;} = 1;
        public int HeartMult {get; set;} = 1;
        public int SubMult {get; set;} = 1;
        public int ShieldMult {get; set;} = 1;
        public int DamageMult {get; set;} = 1;

        private string _description;
        private int _descriptionHash;

        public Task Activate(BattleController battle)
        {
            battle.State.Brain *= BrainMult;
            battle.State.Heart *= HeartMult;
            battle.State.Sub *= SubMult;
            battle.State.Shield *= ShieldMult;
            battle.State.Damage *= DamageMult;

            return Task.CompletedTask;
        }

        public string GetImage(BattleState state) => Image;

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

            LabelFor("Brain", BrainMult);
            LabelFor("Heart", HeartMult);
            LabelFor("Sub", SubMult);
            LabelFor("Shield", ShieldMult);
            LabelFor("TF", DamageMult);

            _description = builder.ToString();
            _descriptionHash = newHash;
            return _description;

            int GetDescriptionHash()
            {
                var hash = new HashCode();
                hash.Add(BrainMult);
                hash.Add(HeartMult);
                hash.Add(SubMult);
                hash.Add(ShieldMult);
                hash.Add(DamageMult);

                return hash.ToHashCode();
            }

            void LabelFor(string resource, int value)
            {
                if (value == 1)
                    return;
                else
                    builder.AppendLine($"{resource}: x{value}");
            }
        }
    }
}