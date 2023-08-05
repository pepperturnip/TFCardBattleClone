using System.Collections.Generic;

namespace TFCardBattle.Core
{
    public static class ListExtensions
    {
        /// <summary>
        /// Removes all items from this list, and adds them to the destination
        /// list.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        /// <typeparam name="TItem"></typeparam>
        public static void TransferAllTo<TItem>(
            this List<TItem> src,
            List<TItem> dst
        )
        {
            dst.AddRange(src);
            src.Clear();
        }
    }
}