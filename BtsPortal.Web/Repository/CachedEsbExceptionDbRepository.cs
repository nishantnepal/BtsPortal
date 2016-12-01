using System.Data.SqlClient;
using BtsPortal.Repositories.Db;
using BtsPortal.Web.Infrastructure.Settings;

namespace BtsPortal.Web.Repository
{
    public class CachedEsbExceptionDbRepository : EsbExceptionDbRepository
    {
        public CachedEsbExceptionDbRepository() : base(dbConnection:new SqlConnection(AppSettings.EsbExceptionDbConnectionString), commandTimeoutSeconds: AppSettings.CommandTimeout)
        {
        }
    }
}