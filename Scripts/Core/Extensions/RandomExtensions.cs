using System;
using System.Collections.Generic;

namespace TFCardBattle.Core
{
    public static class RandomExtensions
    {
        public static T PickFrom<T>(this Random rng, IReadOnlyList<T> items)
        {
            return items[rng.Next(items.Count)];
        }
    }
}