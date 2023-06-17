using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core.ConsumableClasses
{
    /// <summary>
    /// A consumable that just adds resources
    /// </summary>
    public abstract class SimpleConsumable : IConsumable
    {
        public virtual string TexturePath => null;

        public virtual int BrainGain => 0;
        public virtual int HeartGain => 0;
        public virtual int SubGain => 0;
        public virtual int ShieldGain => 0;
        public virtual int Damage => 0;
        public virtual int CardDraw => 0;
        public virtual int SelfHeal => 0;

        public async Task Activate(BattleController battle)
        {
            battle.State.Brain += BrainGain;
            battle.State.Heart += HeartGain;
            battle.State.Sub += SubGain;
            battle.State.Shield += ShieldGain;
            battle.State.Damage += Damage;

            // TODO: play a self-heal animation
            battle.State.PlayerTF -= SelfHeal;

            for (int i = 0; i < CardDraw; i++)
            {
                await battle.DrawCard();
            }
        }
    }
}