using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Cache;
using Valley.RssReader.Core.Services.Interfaces;

namespace Valley.RssReader.Core.Services
{
    public class CachingService<T> : ICachingService<T> where T : class
    {
        public IRuntimeCacheProvider CacheProvider { private get; set; }

        public bool Get(IEnumerable<string> keys, out IEnumerable<T> items)
        {
            string[] keysArray = keys.ToArray();
            T[] cachedItems = keysArray.Select(k => (T)CacheProvider.GetCacheItem(k)).TakeWhile(i => i != null).ToArray();
            items = cachedItems;

            // Indicate if all requested items were retrieved from the cache.
            return cachedItems.Length == keysArray.Length;
        }

        public void Insert(Dictionary<string, T> items)
        {
            foreach (KeyValuePair<string, T> item in items)
            {
                CacheProvider.InsertCacheItem(item.Key, () => item.Value);
            }
        }
    }
}
