using System.Collections.Generic;
using BtsPortal.Entities.Bts;

namespace BtsPortal.Repositories.Interface
{
    public interface ISsoDbRepository
    {
        List<SsoApplicationData> GetSsoApplicationData(string appName, bool isBtdf);
        List<SsoApplication> GetSsoApplications();
        void SaveSsoSetting(string appName, bool isBtdf, string key, string value);
    }
}