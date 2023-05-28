using System;
using System.Threading.Tasks;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class BattleAnimationPlayer : Node, IBattleAnimationPlayer
    {
        public Task DamageEnemy(int damageAmount) => Delay(1);
        public Task DamagePlayer(int damageAmount) => Delay(1);

        private async Task Delay(double seconds)
        {
            var timer = GetTree().CreateTimer(seconds);
            await timer.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        }
    }
}