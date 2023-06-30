using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core.CardEffects
{
    /// <summary>
    /// A card that multiplies the player's current resources by some number.
    /// </summary>
    public class MultiplyResources : ICardEffect
    {
        public int Brain {get; set;} = 1;
        public int Heart{get; set;} = 1;
        public int Sub {get; set;} = 1;
        public int Shield {get; set;} = 1;
        public int Damage {get; set;} = 1;

        private string _description;
        private int _descriptionHash;

        public Task Activate(BattleController battle)
        {
            battle.State.Brain *= Brain;
            battle.State.Heart *= Heart;
            battle.State.Sub *= Sub;
            battle.State.Shield *= Shield;
            battle.State.Damage *= Damage;

            return Task.CompletedTask;
        }

        public string GetOverriddenImage(BattleState state) => null;

        public string GetDescription(BattleState state)
        {
            var builder = new System.Text.StringBuilder();

            LabelFor("Brain", Brain);
            LabelFor("Heart", Heart);
            LabelFor("Sub", Sub);
            LabelFor("Shield", Shield);
            LabelFor("TF", Damage);

            return builder.ToString();

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