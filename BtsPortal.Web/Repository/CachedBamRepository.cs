using System.Collections.Generic;
using System.Data.SqlClient;
using BtsPortal.Cache;
using BtsPortal.Entities.Bam;
using BtsPortal.Repositories.Db;
using BtsPortal.Web.Infrastructure.Settings;

namespace BtsPortal.Web.Repository
{
    public class CachedBamRepository : BamRepository
    {
        private readonly ICacheProvider _cacheProvider;

        public CachedBamRepository(ICacheProvider cacheProvider) : base(dbConnection:new SqlConnection(AppSettings.BamDatabaseConnectionString))
        {
            _cacheProvider = cacheProvider;
        }

        public override List<Activity> LoadActivities()
        {
            List<Activity> vm;
            object cached = _cacheProvider.Get(CacheKey.Bam_Activities.ToString());
            if (cached == null)
            {
                vm = base.LoadActivities();
                if (vm.Count > 0) _cacheProvider.Set(CacheKey.Bam_Activities.ToString(), vm);

            }
            else
            {
                vm = (List<Activity>)cached;
            }

           
            return vm;
        }


    }
}