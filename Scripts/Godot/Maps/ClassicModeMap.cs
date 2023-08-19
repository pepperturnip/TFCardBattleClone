using System;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class ClassicModeMap : Control
    {
        private PlayerLoadoutSelection _loadoutSelectionPage => GetNode<PlayerLoadoutSelection>("%LoadoutSelectionPage");
        private BattleUI _battlePage => GetNode<BattleUI>("%BattlePage");

        public override void _Ready()
        {
            ChangePage(_loadoutSelectionPage);
        }

        private void ChangePage(Control page)
        {
            // Hide all pages
            for (int i = 0; i < GetChildCount(); i++)
            {
                var child = GetChild<Control>(i);
                child.ProcessMode = ProcessModeEnum.Disabled;
                child.Visible = false;
            }

            // Unhide the one page we're going to
            page.ProcessMode = ProcessModeEnum.Inherit;
            page.Visible = true;
        }
    }
}