using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BtsPortal.Cache;
using BtsPortal.Entities.Bts;
using BtsPortal.Repositories.Db;
using BtsPortal.Web.Infrastructure.Settings;

namespace BtsPortal.Web.Repository
{
    internal class CachedBizTalkMgmtRepository : BizTalkMgmtRepository
    {
        private readonly ICacheProvider _cacheProvider;
        public CachedBizTalkMgmtRepository(ICacheProvider cacheProvider) : base(new SqlConnection(AppSettings.BtsMgmtBoxDatabaseConnectionString))
        {
            _cacheProvider = cacheProvider;

        }

        public override List<BtsArtifact> LoadArtifactTypes()
        {
            List<BtsArtifact> vm;
            object cached = _cacheProvider.Get(CacheKey.Bts_ApplicationArtifacts.ToString());
            if (cached == null)
            {
                vm = base.LoadArtifactTypes();
                if (vm.Count > 0) _cacheProvider.Set(CacheKey.Bts_ApplicationArtifacts.ToString(), vm);
            }
            else
            {
                vm = (List<BtsArtifact>)cached;
            }

            return vm;
        }

        public override List<BtsParty> GetParties()
        {
            string key = CacheKey.Bts_Edi_Parties.ToString();
            List<BtsParty> vm;
            object cached = _cacheProvider.Get(key);
            if (cached == null)
            {
                vm = base.GetParties();
                if (vm.Count > 0) _cacheProvider.Set(key, vm);
            }
            else
            {
                vm = (List<BtsParty>)cached;
            }

            return vm;
        }

    }
}
