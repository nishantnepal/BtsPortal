using System.Collections.Generic;
using System.Data.SqlClient;
using BtsPortal.Cache;
using BtsPortal.Entities.Bts;
using BtsPortal.Repositories.Db;
using BtsPortal.Web.Infrastructure.Settings;

namespace BtsPortal.Web.Repository
{
    public class CachedBizTalkMsgBoxRepository : BizTalkMsgBoxRepository
    {
        private readonly ICacheProvider _cacheProvider;
        public CachedBizTalkMsgBoxRepository(ICacheProvider cacheProvider)
                                    : base(dbConnection:new SqlConnection(AppSettings.BtsMsgBoxDatabaseConnectionString), commandTimeoutSeconds: AppSettings.CommandTimeout)
        {
            _cacheProvider = cacheProvider;
        }

        public override List<BtsModule> LoadApplicationModules()
        {
            List<BtsModule> vm;
            object cached = _cacheProvider.Get(CacheKey.Bts_MsgBox_AppModules.ToString());
            if (cached == null)
            {
                vm = base.LoadApplicationModules();
                if (vm.Count > 0) _cacheProvider.Set(CacheKey.Bts_MsgBox_AppModules.ToString(), vm);
            }
            else
            {
                vm = (List<BtsModule>)cached;
            }

            return vm;
        }
    }
}

