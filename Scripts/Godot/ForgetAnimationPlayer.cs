using System;
using System.Collections.Generic;
using Godot;
using TFCardBattle.Core;

namespace TFCardBattle.Godot
{
    public partial class ForgetAnimationPlayer : Node2D
    {
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%Animator");
        private CardModel _model => GetNode<CardModel>("%CardModel");

        private Queue<ICard> _cardsToForget = new Queue<ICard>();
        private BattleState _battleState;

        public override void _Process(double delta)
        {
            if (_cardsToForget.Count > 0 && !_animator.IsPlaying())
            {
                _model.SetCard(_cardsToForget.Dequeue(), _battleState);
                _animator.ResetAndPlay("Forget");
            }
        }

        public void QueueForget(ICard card, BattleState state)
        {
            _cardsToForget.Enqueue(card);
            _battleState = state;
        }
    }
}