using System.Collections.Generic;
using System.Data.SqlClient;
using BtsPortal.Cache;
using BtsPortal.Entities.Bts;
using BtsPortal.Repositories.Db;
using BtsPortal.Web.Infrastructure.Settings;

namespace BtsPortal.Web.Repository
{
    public class CachedSsoDbRepository : SsoDbRepository
    {
        private readonly ICacheProvider _cacheProvider;
        public CachedSsoDbRepository(ICacheProvider cacheProvider) : base(dbConnection:new SqlConnection(AppSettings.SsoDbConnectionString), commandTimeoutSeconds: AppSettings.CommandTimeout)
        {
            _cacheProvider = cacheProvider;
        }

        public override List<SsoApplication> GetSsoApplications()
        {
            List<SsoApplication> vm;
            object cached = _cacheProvider.Get(CacheKey.Sso_Applications.ToString());
            if (cached == null)
            {
                vm = base.GetSsoApplications();
                if (vm.Count > 0) _cacheProvider.Set(CacheKey.Sso_Applications.ToString(), vm);
            }
            else
            {
                vm = (List<SsoApplication>)cached;
            }


            return vm;
        }
    }
}


