namespace IA.Bts.Portal.Infrastructure.Cache
{
    public enum CacheKey
    {
        Sso_Applications,
        Bts_ApplicationArtifacts,
        Bts_Edi_Parties,
        Bts_MsgBox_AppModules,
        Bts_Applications,
    }

    public interface ICacheProvider
    {
        object Get(CacheKey key);
        void Set(CacheKey key, object data);
        void Remove(CacheKey key);
    }
}
