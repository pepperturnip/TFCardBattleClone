using System;
using Godot;

namespace TFCardBattle.Godot
{
    public partial class ClassicModeMap : Control
    {
        private LoadoutSelectionPage _loadoutSelectionPage => GetNode<LoadoutSelectionPage>("%LoadoutSelectionPage");
        private BattlePage _battlePage => GetNode<BattlePage>("%BattlePage");

        public override void _Ready()
        {
            ChangePage(_loadoutSelectionPage);
        }

        public void OnLoadoutSelected()
        {
            ChangePage(_battlePage);
            _battlePage.StartBattle(_loadoutSelectionPage.SelectedLoadout);
        }

        public void OnBattleEnded(bool playerWon)
        {
            Maps.Instance.GoToTitleScreen();
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