using System;
using System.Runtime.Caching;

namespace BtsPortal.Cache
{
    public class MemoryCacheProvider : ICacheProvider
    {
        public object Get(string key)
        {
            return MemoryCache.Default.Get(key);
        }

        public void Set(string key, object data)
        {
            MemoryCache.Default.Set(key, data, new DateTimeOffset(DateTime.Now.AddDays(1)));
        }

        public void Remove(string key)
        {
            MemoryCache.Default.Remove(key);
        }
    }
}
