using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BtsPortal.Cache;
using BtsPortal.Entities.Bam;
using BtsPortal.Repositories.Db;
using BtsPortal.Web.Infrastructure.Settings;

namespace BtsPortal.Web.Repository
{
    public class CachedBtsPortalRepository : BtsPortalRepository
    {
        private readonly ICacheProvider _cacheProvider;
        public CachedBtsPortalRepository(ICacheProvider cacheProvider) : base(dbConnection:new SqlConnection(AppSettings.BtsPortalDatabaseConnectionString))
        {
            _cacheProvider = cacheProvider;
        }

        public override List<ActivityView> LoadActivitiesViews(bool activeOnly = false)
        {
            List<ActivityView> vm;
            object cached = _cacheProvider.Get(CacheKey.IaBts_BamViews.ToString());
            if (cached == null)
            {
                vm = base.LoadActivitiesViews();
                if (vm.Count > 0) _cacheProvider.Set(CacheKey.IaBts_BamViews.ToString(), vm);
            }
            else
            {
                vm = (List<ActivityView>)cached;
            }

            if (activeOnly)
            {
                vm = vm.Where(m => m.IsActive).ToList();
            }
            return vm;
        }

        public override ActivityView LoadActivityView(int id)
        {
            ActivityView vm;
            string key = string.Concat(CacheKey.IaBts_BamView.ToString(), "_", id);
            object cached = _cacheProvider.Get(key);
            if (cached == null)
            {
                vm = base.LoadActivityView(id);
                _cacheProvider.Set(key, vm);
            }
            else
            {
                vm = (ActivityView)cached;
            }

            return vm;
        }
    }
}