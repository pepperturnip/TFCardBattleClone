using System;
using System.Threading.Tasks;

namespace TFCardBattle.Core
{
    public interface IBattleAnimationPlayer
    {
        Task DamagePlayer(int damageAmount);
        Task DamageEnemy(int damageAmount);
        Task DrawCard(Card card);
        Task PlayCard(int handIndexPlayed, BattleState newState);
        Task DiscardResources();
        Task DiscardHand();
        Task RefreshBuyPile(Card[] cards);
        Task BuyCard(int buyPileIndex, bool isPermanentBuyPileCard);
        Task ForgetCard(Card card, BattleState state);
        Task BossRoundStart();
    }
}