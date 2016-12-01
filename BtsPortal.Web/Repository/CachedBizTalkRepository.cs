using System.Collections.Generic;
using System.Data.SqlClient;
using BtsPortal.Cache;
using BtsPortal.Entities.Bts;
using BtsPortal.Repositories.Db;
using BtsPortal.Web.Infrastructure.Settings;

namespace BtsPortal.Web.Repository
{
    public class CachedBizTalkRepository : BizTalkRepository
    {
        private readonly ICacheProvider _cacheProvider;
        public CachedBizTalkRepository(ICacheProvider cacheProvider) : base(dbConnection:new SqlConnection(AppSettings.BtsMgmtBoxDatabaseConnectionString))
        {
            _cacheProvider = cacheProvider;
        }

        public override List<BtsApplication> GetBiztalkApplications()
        {
            List<BtsApplication> vm;
            object cached = _cacheProvider.Get(CacheKey.Bts_Applications.ToString());
            if (cached == null)
            {
                vm = base.GetBiztalkApplications();
                if (vm.Count > 0) _cacheProvider.Set(CacheKey.Bts_Applications.ToString(), vm);
            }
            else
            {
                vm = (List<BtsApplication>)cached;
            }

            return vm;
        }
    }
}