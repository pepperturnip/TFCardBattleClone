using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TFCardBattle.Core.Parsing
{
    using ClassCache = Dictionary<string, Type>;

    public static class FindClass
    {
        private static Dictionary<string, ClassCache> _caches
            = new Dictionary<string, ClassCache>();

        public static Type InNamespace(string nameSpace, string className)
        {
            if (!_caches.ContainsKey(nameSpace))
                _caches[nameSpace] = new ClassCache();

            if (_caches[nameSpace].TryGetValue(className, out var result))
                return result;

            // Only search for classes in the the given namespace, for
            // security.  We don't want nefarious dudes instantiating any C#
            // class they want!
            var type = Assembly.GetExecutingAssembly()
                .DefinedTypes
                .Where(t => t.Namespace == nameSpace)
                .FirstOrDefault(t => t.Name == className);

            _caches[nameSpace][className] = type;
            return type;
        }
    }
}