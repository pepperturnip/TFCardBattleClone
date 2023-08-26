using System.Threading.Tasks;

namespace TFCardBattle.Core.LingeringEffects
{
    public class HourglassWaist : ILingeringEffect
    {
        public Task OnCardFinishedActivating(BattleController battle, Card card)
        {
            // TODO: do this in a less hacky way.
            // The original game actually replaces Mysterious Pills with an
            // upgraded version.  That's what we should do, too.
            if (card.Name == "Mysterious Pills")
                battle.State.Damage++;

            return Task.CompletedTask;
        }
    }
}