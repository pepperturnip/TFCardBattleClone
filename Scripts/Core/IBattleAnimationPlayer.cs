using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core
{
    public interface IBattleAnimationPlayer
    {
        Task DamagePlayer(int damageAmount);
        Task DamageEnemy(int damageAmount);
        Task DrawCard(BattleState state);
        Task PlayCard(int handIndexPlayed, BattleState newState);
        Task DiscardHand();
    }
}