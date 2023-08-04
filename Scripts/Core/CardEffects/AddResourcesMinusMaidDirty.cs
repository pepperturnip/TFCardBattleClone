using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    /// <summary>
    /// Adds the given amount of each resource, minus the MaidDirty resource.
    /// Resources that aren't included in the JSON(IE: are null in the C# class)
    /// will not be changed.
    /// </summary>
    public class AddResourcesMinusMaidDirty : ICardEffect
    {
        public int? Brain {get; set;}
        public int? Heart {get; set;}
        public int? Sub {get; set;}
        public int? Shield {get; set;}
        public int? Damage {get; set;}
        public int? Draw {get; set;}
        public int? SelfHeal {get; set;}

        public async Task Activate(BattleController battle)
        {
            int dirty = (int)battle.State.CustomResources["MaidDirty"];

            if (Brain.HasValue)
                battle.State.Brain += Brain.Value - dirty;

            if (Heart.HasValue)
                battle.State.Heart += Heart.Value - dirty;

            if (Sub.HasValue)
                battle.State.Sub += Sub.Value - dirty;

            if (Shield.HasValue)
                battle.State.Shield += Shield.Value - dirty;

            if (Damage.HasValue)
                battle.State.Damage += Damage.Value - dirty;

            if (SelfHeal.HasValue)
                battle.State.PlayerTF -= SelfHeal.Value - dirty;

            if (Draw.HasValue)
            {
                for (int i = 0; i < (Draw - dirty); i++)
                {
                    await battle.DrawCard();
                }
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

            return builder.ToString();

            void LabelFor(string resource, int? value)
            {
                if (value == 0)
                    return;
                else if (value > 0)
                    builder.AppendLine($"{resource}: +({value} - Trash Accumulated)");
                else
                    builder.AppendLine($"{resource}: -({Math.Abs(value.Value)} + Trash Accumulated)");
            }
        }

        public virtual string GetOverriddenImage(BattleState state) => null;
    }
}