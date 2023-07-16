using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core
{
    public interface ILingeringEffect
    {
        Task OnCardAboutToActivate(BattleController battle, Card card) => Task.CompletedTask;
        Task OnCardFinishedActivating(BattleController battle, Card card) => Task.CompletedTask;

        Task OnConsumableAboutToActivate(BattleController battle, IConsumable consumable) => Task.CompletedTask;
        Task OnConsumableFinishedActivating(BattleController battle, IConsumable consumable) => Task.CompletedTask;

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

        Task OnTurnEnd(BattleController battle) => Task.CompletedTask;
    }
}