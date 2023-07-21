using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    /// <summary>
    /// A card that merely grants resources, without any other special effects.
    /// </summary>
    public class Simple : ICardEffect
    {
        public int Brain {get; set;}
        public int Heart {get; set;}
        public int Sub {get; set;}
        public int Shield {get; set;}
        public int Damage {get; set;}
        public int Draw {get; set;}
        public int SelfHeal {get; set;}

        public ConsumableId[] Consumables {get; set;} = Array.Empty<ConsumableId>();

        public IReadOnlyDictionary<CustomResourceId, double> CustomResources {get; set;}
            = new Dictionary<CustomResourceId, double>();

        public async Task Activate(BattleController battle)
        {
            battle.State.Brain += Brain;
            battle.State.Heart += Heart;
            battle.State.Sub += Sub;
            battle.State.Shield += Shield;
            battle.State.Damage += Damage;

            foreach (var id in CustomResources.Keys)
            {
                battle.State.CustomResources[id] += CustomResources[id];
            }

            foreach (var consumableId in Consumables)
            {
                Consumable consumable = battle.State.CardRegistry.Consumables[consumableId];
                await battle.AddConsumable(consumable);
            }

            // TODO: play a self-heal animation
            battle.State.PlayerTF -= SelfHeal;

            for (int i = 0; i < Draw; i++)
            {
                await battle.DrawCard();
            }
        }

        public string GetDescription(BattleState state)
        {
            var builder = new System.Text.StringBuilder();

            LabelFor("Brain", Brain);
            LabelFor("Heart", Heart);
            LabelFor("Sub", Sub);
            LabelFor("Shield", Shield);
            LabelFor("TF", Damage);
            LabelFor("Draw", Draw);
            LabelFor("Self heal", SelfHeal);

            foreach (var id in CustomResources.Keys)
            {
                string resourceName = state.CardRegistry.CustomResources[id].Name;
                LabelForDouble(resourceName, CustomResources[id]);
            }

            foreach (var c in Consumables)
            {
                builder.AppendLine($"+1 {c.GetType().Name}");
            }

            return builder.ToString();

            void LabelFor(string resource, int value)
            {
                if (value == 0)
                    return;
                else if (value > 0)
                    builder.AppendLine($"{resource}: +{value}");
                else
                    builder.AppendLine($"{resource}: {value}");
            }

            void LabelForDouble(string resource, double value)
            {
                if (value == 0)
                    return;
                else if (value > 0)
                    builder.AppendLine($"{resource}: +{value}");
                else
                    builder.AppendLine($"{resource}: {value}");
            }
        }

        public virtual string GetOverriddenImage(BattleState state) => null;
    }
}