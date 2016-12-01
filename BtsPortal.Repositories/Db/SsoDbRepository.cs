using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BtsPortal.Entities.Bts;
using BtsPortal.Repositories.Helpers.Sso;
using BtsPortal.Repositories.Interface;
using Dapper;

namespace BtsPortal.Repositories.Db
{
    public class SsoDbRepository : ISsoDbRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly int _commandTimeoutSeconds;
        public SsoDbRepository(IDbConnection dbConnection, int commandTimeoutSeconds = 600)
        {
            _dbConnection = dbConnection;
            _commandTimeoutSeconds = commandTimeoutSeconds;
        }

        public virtual List<SsoApplication> GetSsoApplications()
        {
            const string query = @"SELECT distinct ai_app_name as Name,ai_contact_info as ContactInfo
                                        from [dbo].[SSOX_ApplicationInfo] (NOLOCK)
                                         WHERE ai_app_name not LIKE '{%'";
            var vm = _dbConnection.Query<SsoApplication>(query).ToList();
            return vm;
        }

        public List<SsoApplicationData> GetSsoApplicationData(string appName, bool isBtdf)
        {
            var vm = new List<SsoApplicationData>();
            if (isBtdf)
            {
                vm = BtdfManager.LoadBtdfSettingsFromSso(appName);
            }
            else
            {
                string description;
                string contactInfo;
                string appUserAcct;
                string appAdminAcct;
                HybridDictionary props = SSOConfigManager.GetConfigProperties(appName, out description, out contactInfo, out appUserAcct, out appAdminAcct);
                vm.AddRange(from DictionaryEntry appProp in props
                            select new SsoApplicationData()
                            {
                                Key = appProp.Key.ToString(),
                                Value = appProp.Value.ToString()
                            });
            }

            return vm;
        }

        public void SaveSsoSetting(string appName, bool isBtdf, string key, string value)
        {
            if (isBtdf)
            {
                BtdfManager.SaveBtdfSetting(appName, key, value);
            }
            else
            {
                SSOClientHelper.Write(appName, key, value);
            }
        }


    }
}