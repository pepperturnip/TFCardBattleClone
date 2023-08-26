using System;
using System.Linq;
using Godot;
using Newtonsoft.Json;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class EndlessModeMap : Control
    {
        private PlayerLoadout _playerLoadout;
        private int _winStreak = 0;

        private TransformationSelectionPage _tfSelectionPage => GetNode<TransformationSelectionPage>("%TransformationSelectionPage");
        private RelicSelectionPage _relicSelectionPage => GetNode<RelicSelectionPage>("%RelicSelectionPage");
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

        public void GoToRelicPage()
        {
            _relicSelectionPage.Init(_playerLoadout);
            ChangePage(_relicSelectionPage);
        }

        public void GoToThemePackPage()
        {
            _packSelectionPage.Init(_playerLoadout);
            ChangePage(_packSelectionPage);
        }

        public void StartBattle()
        {
            _packSelectionPage.ShowBackButton = false;

            var enemyLoadout = new EnemyLoadout
            {
                MaxTF = 100 + (_winStreak * 10),

                MinDamageOffset = 2,
                MinDamageSlopeRise = 1 + _winStreak,
                MinDamageSlopeRun = 12,

                MaxDamageOffset = 3,
                MaxDamageSlopeRise = 1 + _winStreak,
                MaxDamageSlopeRun = 6
            };

            _battlePage.StartBattle(_playerLoadout, enemyLoadout);
            ChangePage(_battlePage);
        }

        public void OnBattleEnded(bool playerWon)
        {
            if (!playerWon)
            {
                Maps.Instance.GoToTitleScreen();
                return;
            }

            _winStreak++;
            GoToRelicPage();
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