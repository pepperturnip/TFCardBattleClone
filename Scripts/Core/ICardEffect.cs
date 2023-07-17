using System.Threading.Tasks;

namespace TFCardBattle.Core
{
    public interface ICardEffect
    {
        Task Activate(BattleController battle);

        string GetDescription(BattleState state);
        string GetOverriddenImage(BattleState state);
    }
}