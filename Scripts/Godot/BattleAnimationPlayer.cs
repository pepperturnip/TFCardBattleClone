using System;
using System.Threading.Tasks;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public class BattleAnimationPlayer : IBattleAnimationPlayer
    {
        public Task DamageEnemy(int damageAmount) => Task.Delay(1000);
        public Task DamagePlayer(int damageAmount) => Task.Delay(1000);
    }
}