
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class BattlePage : Control
    {
        [Signal] public delegate void BattleEndedEventHandler(bool playerWon);

        [Export] public CardModelFactory CardModelFactory;

        private TFBar _playerTFBar => GetNode<TFBar>("%PlayerTFBar");
        private TFBar _enemyTFBar => GetNode<TFBar>("%EnemyTFBar");

        private Label _enemyDamageRangeLabel => GetNode<Label>("%EnemyDamageRangeLabel");

        private HandDisplay _handDisplay => GetNode<HandDisplay>("%HandDisplay");
        private BuyPileDisplay _buyPileDisplay => GetNode<BuyPileDisplay>("%BuyPileDisplay");

        private ResourceCounter _bossDamageCounter => GetNode<ResourceCounter>("%BossDamageCounter");
        private ResourcesDisplay _resourcesDisplay => GetNode<ResourcesDisplay>("%ResourcesDisplay");
        private ConsumablesDisplay _consumablesDisplay => GetNode<ConsumablesDisplay>("%ConsumablesDisplay");

        private CardListDisplay _discardPilePanelContents => GetNode<CardListDisplay>("%DiscardPileDisplay");
        private CardListDisplay _deckPanelContents => GetNode<CardListDisplay>("%DeckDisplay");
        private CardListDisplay _inPlayPanelContents => GetNode<CardListDisplay>("%InPlayCardsDisplay");
        private CardButton _showDiscardButton => GetNode<CardButton>("%ShowDiscardButton");
        private CardButton _showInPlayButton => GetNode<CardButton>("%ShowInPlayButton");

        private BattleController Battle;

        public async void StartBattle(PlayerLoadout loadout)
        {
            Battle = new BattleController(
                loadout: loadout,
                rng: new Random((int)DateTimeOffset.Now.ToUnixTimeMilliseconds()),
                animationPlayer: GetNode<BattleAnimationPlayer>("%BattleAnimationPlayer")
            );

            this.CardModelFactory.SetBattleState(Battle.State);
            _buyPileDisplay.SetBattle(Battle);
            _handDisplay.SetBattleState(Battle.State);

            await Battle.StartTurn();
            RefreshDisplay();
        }

        public override void _Process(double delta)
        {
            if (Battle.BattleEnded)
            {
                var state = Battle.State;

                bool playerWon =
                    state.IsBossRound &&
                    state.EnemyTF >= state.EnemyMaxTF &&
                    state.PlayerTF < state.PlayerMaxTF;

                EmitSignal(SignalName.BattleEnded, playerWon);
            }
        }

        public void OnIsAnimatingChanged(bool isAnimating) => EnableInput(!isAnimating);

        public async void OnEndTurnClicked()
        {
            await Battle.EndTurn();
            RefreshDisplay();
        }

        public async void OnPlayCardClicked(int handIndex)
        {
            await Battle.PlayCard(handIndex);
            RefreshDisplay();
        }

        public async void OnBuyCardClicked(int buyPileIndex)
        {
            await Battle.BuyCard(buyPileIndex);
            RefreshDisplay();
        }

        public async void OnUseConsumableClicked(int index)
        {
            await Battle.UseConsumable(index);
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            this.CardModelFactory.SetBattleState(Battle.State);

            _enemyDamageRangeLabel.Text = $"{Battle.EnemyMinTFDamage} - {Battle.EnemyMaxTFDamage}";

            _playerTFBar.MaxValue = Battle.State.PlayerMaxTF;
            _playerTFBar.Value = Battle.State.PlayerTF;

            _enemyTFBar.MaxValue = Battle.State.EnemyMaxTF;
            _enemyTFBar.Value = Battle.State.EnemyTF;

            _handDisplay.Refresh();
            _buyPileDisplay.Refresh();

            _resourcesDisplay.UpdateResources(Battle.State);
            _consumablesDisplay.Refresh(Battle.State.Consumables.ToArray());
            _bossDamageCounter.AccumulateToValue(
                Battle.State.IsBossRound
                    ? Battle.TotalDamageToBoss()
                    : 0
            );

            _discardPilePanelContents.Refresh(Battle.State.Discard);
            _deckPanelContents.Refresh(Battle.State.Deck);
            _inPlayPanelContents.Refresh(Battle.State.InPlay);

            _showDiscardButton.SetCard(Battle.State.Discard.LastOrDefault(), Battle.State);
            _showInPlayButton.SetCard(Battle.State.InPlay.LastOrDefault(), Battle.State);

            _buyPileDisplay.Visible = !Battle.State.IsBossRound;
        }

        private void EnableInput(bool enabled)
        {
            _handDisplay.EnableInput = enabled;
            _buyPileDisplay.EnableInput = enabled;
            GetNode<Button>("%EndTurnButton").Disabled = !enabled;
        }
    }
}