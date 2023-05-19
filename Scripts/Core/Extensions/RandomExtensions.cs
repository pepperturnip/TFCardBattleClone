using System;
using System.Collections.Generic;
using System.Linq;

namespace TFCardBattle.Core
{
    public static class RandomExtensions
    {
        public static T PickFrom<T>(this Random rng, IReadOnlyList<T> items)
        {
            return items[rng.Next(items.Count)];
        }

        public static T PickFromWeighted<T>(
            this Random rng,
            params (T value, int weight)[] weights
        )
        {
            int maxRoll = weights
                .Select(w => w.weight)
                .Sum();

            int roll = rng.Next(maxRoll);

            int sum = 0;
            foreach (var w in weights)
            {
                sum += w.weight;
                if (roll < sum)
                    return w.value;
            }

            throw new Exception("Uhh...I didn't think this through, apparently");
        }
    }
}