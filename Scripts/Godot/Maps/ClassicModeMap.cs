using System;
using System.Linq;
using Godot;
using Newtonsoft.Json;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class ClassicModeMap : Control
    {
        private PlayerLoadout _playerLoadout;

        private TransformationSelectionPage _tfSelectionPage => GetNode<TransformationSelectionPage>("%TransformationSelectionPage");
        private ThemePackSelectionPage _packSelectionPage => GetNode<ThemePackSelectionPage>("%ThemePackSelectionPage");
        private BattlePage _battlePage => GetNode<BattlePage>("%BattlePage");

        public override void _Ready()
        {
            string defaultPacksJson = FileAccess.GetFileAsString("res://Content/DefaultLoadout.json");
            var defaultPacks = JsonConvert.DeserializeObject<CardPackId[]>(defaultPacksJson)
                .Select(id => ContentRegistry.CardPacks[id])
                .ToArray();

            _playerLoadout = new PlayerLoadout
            {
                ThemePacks = defaultPacks,
                PermanentBuyPile = ContentRegistry.CardPacks["StandardPermanentBuyPile"].Cards.Values,
                StartingDeck = PlayerStartingDeck.StartingDeck()
            };

            GoToTransformationPage();
        }

        public void GoToTransformationPage()
        {
            _tfSelectionPage.Init(_playerLoadout);
            ChangePage(_tfSelectionPage);
        }

        public void GoToThemePackPage()
        {
            _packSelectionPage.Init(_playerLoadout);
            ChangePage(_packSelectionPage);
        }

        public void StartBattle()
        {
            _battlePage.StartBattle(_playerLoadout, new EnemyLoadout());
            ChangePage(_battlePage);
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