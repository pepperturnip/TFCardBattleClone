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

        public override void _Process(double delta)
        {
            if (_cardsToForget.Count > 0 && !_animator.IsPlaying())
            {
                _model.Card = _cardsToForget.Dequeue();
                _animator.ResetAndPlay("Forget");
            }
        }

        public void QueueForget(ICard card)
        {
            _cardsToForget.Enqueue(card);
        }
    }
}