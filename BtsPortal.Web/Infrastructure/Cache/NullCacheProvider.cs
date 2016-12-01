namespace IA.Bts.Portal.Infrastructure.Cache
{
    public class NullCacheProvider:ICacheProvider
    {
        public object Get(CacheKey key)
        {
            return null;
        }

        public void Set(CacheKey key, object data)
        {
            return;
        }

        public void Remove(CacheKey key)
        {
            return;
        }
    }
}
