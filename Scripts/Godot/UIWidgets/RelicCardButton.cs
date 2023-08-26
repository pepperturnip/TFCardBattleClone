using System;
using TFCardBattle.Core;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class RelicCardButton : HoverGrowButton
    {
        public void SetRelic(RelicId relicId)
        {
            var model = GetChild<RelicSelectCardModel>(0);
            model.SetRelic(relicId);
        }
    }
}