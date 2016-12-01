using System;
using System.Web;

namespace BtsPortal.Cache
{
    public class HttpCacheProvider : ICacheProvider
    {
        public object Get(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }

        public void Set(string key, object data)
        {
            HttpRuntime.Cache.Insert(key, data, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromDays(1));
        }

        public void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }
    }
}
