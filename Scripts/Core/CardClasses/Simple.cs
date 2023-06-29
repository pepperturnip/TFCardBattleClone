using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardClasses
{
    /// <summary>
    /// A card that merely grants resources, without any other special effects.
    /// </summary>
    public class Simple : ICard
    {
        public string Name {get; set;}
        public string Desc => GetDescription();
        public string Image {get; set;}
        public string[] Gifs {get; set;}
        public CardPurchaseStats PurchaseStats {get; set;}
        public bool DestroyOnActivate {get; set;}

        public int Brain {get; set;}
        public int Heart {get; set;}
        public int Sub {get; set;}
        public int Shield {get; set;}
        public int Damage {get; set;}
        public int Draw {get; set;}
        public int SelfHeal {get; set;}
        public IConsumable[] Consumables {get; set;} = Array.Empty<IConsumable>();



        private string _description;
        private int _descriptionHash;

        public async Task Activate(BattleController battle)
        {
            battle.State.Brain += Brain;
            battle.State.Heart += Heart;
            battle.State.Sub += Sub;
            battle.State.Shield += Shield;
            battle.State.Damage += Damage;

            foreach (var consumable in Consumables)
            {
                await battle.AddConsumable(consumable);
            }

            // TODO: play a self-heal animation
            battle.State.PlayerTF -= SelfHeal;

            for (int i = 0; i < Draw; i++)
            {
                await battle.DrawCard();
            }
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

            LabelFor("Brain", Brain);
            LabelFor("Heart", Heart);
            LabelFor("Sub", Sub);
            LabelFor("Shield", Shield);
            LabelFor("TF", Damage);
            LabelFor("Draw", Draw);
            LabelFor("Self heal", SelfHeal);

            foreach (var c in Consumables)
            {
                builder.AppendLine($"+1 {c.GetType().Name}");
            }

            _description = builder.ToString();
            _descriptionHash = newHash;
            return _description;

            int GetDescriptionHash()
            {
                var hash = new HashCode();
                hash.Add(Brain);
                hash.Add(Heart);
                hash.Add(Sub);
                hash.Add(Shield);
                hash.Add(Damage);
                hash.Add(Draw);

                foreach (var c in Consumables)
                {
                    hash.Add(c.GetType().Name);
                }

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