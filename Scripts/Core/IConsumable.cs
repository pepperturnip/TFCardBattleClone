using System.Threading.Tasks;

namespace TFCardBattle.Core
{
    public interface IConsumable
    {
        string Image {get;}
        Task Activate(BattleController battle);
    }
}