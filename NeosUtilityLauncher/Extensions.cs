using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JanoschR.NeosUtilityLauncher {
    public static class Extensions {
        
        public static void Increment <TKey> (this IDictionary<TKey, int> dictionary, TKey key, int initial = 0) {
            if (dictionary.ContainsKey(key)) {
                dictionary[key]++;
            } else {
                dictionary.Add(key, initial + 1);
            }
        }

        public static void Decrement <TKey>(this IDictionary<TKey, int> dictionary, TKey key, int initial = 0) {
            if (dictionary.ContainsKey(key)) {
                dictionary[key]--;
            } else {
                dictionary.Add(key, initial - 1);
            }
        }

        public static TValue Get <TKey, TValue> (this IDictionary<TKey, TValue> dictionary, TKey key, TValue _default = default) {
            if (dictionary.ContainsKey(key)) {
                return dictionary[key];
            } else {
                return _default;
            }
        }
    }
}
