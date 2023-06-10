using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
namespace TFCardBattle.Godot
{
    public static class NodeExtensions
    {
        public static IEnumerable<TNode> EnumerateChildren<TNode>(this Node parent) where TNode : Node
        {
            for (int i = 0; i < parent.GetChildCount(); i++)
                yield return parent.GetChild<TNode>(i);
        }
    }
}