using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core
{
    public interface ILingeringEffect
    {
        /// <summary>
        /// Called when damage is about to be dealt to the player.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="damage">
        ///     The amount of damage the player is about to receive, after
        ///     shields have been subtracted.
        ///
        ///     This is a ref parameter, so you can change it to modify the
        ///     damage received at the last second.
        /// </param>
        Task OnPlayerAboutToTakeDamage(BattleController battle, ref int damage) => Task.CompletedTask;
    }
}