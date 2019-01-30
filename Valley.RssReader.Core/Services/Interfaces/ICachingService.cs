using System.Collections.Generic;
using Umbraco.Core.Cache;

namespace Valley.RssReader.Core.Services.Interfaces
{
    public interface ICachingService<T>
    {
        IRuntimeCacheProvider CacheProvider { set; }

        bool Get(IEnumerable<string> keys, out IEnumerable<T> items);

        void Insert(Dictionary<string, T> items);
    }
}
