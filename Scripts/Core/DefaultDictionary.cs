using System.Collections.Generic;

namespace TFCardBattle.Core
{
    public class DefaultDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _content = new Dictionary<TKey, TValue>();
        private readonly TValue _default;

        public DefaultDictionary(TValue defaultValue)
        {
            _default = defaultValue;
        }

        public TValue this[TKey key]
        {
            get => _content.GetValueOrDefault(key);
            set => _content[key] = value;
        }
    }
}