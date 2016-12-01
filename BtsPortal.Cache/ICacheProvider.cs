namespace BtsPortal.Cache
{
   public interface ICacheProvider
    {
        object Get(string key);
        void Set(string key, object data);
        void Remove(string key);
    }
}
