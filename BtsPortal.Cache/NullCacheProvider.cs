namespace BtsPortal.Cache
{
    public class NullCacheProvider:ICacheProvider
    {
        public object Get(string key)
        {
            return null;
        }

        public void Set(string key, object data)
        {
            return;
        }

        public void Remove(string key)
        {
            return;
        }
    }
}
