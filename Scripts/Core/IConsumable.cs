using System.Threading.Tasks;

namespace TFCardBattle.Core
{
    public interface IConsumable
    {
        string TexturePath {get;}
        Task Activate(BattleController battle);
    }
}