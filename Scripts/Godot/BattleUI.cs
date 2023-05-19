
using System;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class BattleUI : Control
    {
        public BattleController Battle = new BattleController(
            PlaceholderCards.AutoGenerateCatalog(),
            PlaceholderCards.StartingDeck(),
            new Random((int)DateTimeOffset.Now.ToUnixTimeMilliseconds())
        );

        [Export] public int PlayerTF
        {
            get => Battle.State.PlayerTF;
            set {}
        }

        public void OnEndTurnClicked()
        {
            Battle.EndTurn();
        }
    }
}