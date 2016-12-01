using System;
using System.Web;

namespace IA.Bts.Portal.Infrastructure.Cache
{
    public class HttpCacheProvider : ICacheProvider
    {
        public object Get(CacheKey key)
        {
            return HttpRuntime.Cache.Get(key.ToString());
        }

        public void Set(CacheKey key, object data)
        {
            HttpRuntime.Cache.Insert(key.ToString(), data, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromDays(1));
        }

        public void Remove(CacheKey key)
        {
            HttpRuntime.Cache.Remove(key.ToString());
        }
    }
}
