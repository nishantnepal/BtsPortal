using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BtsPortal.Entities.Bts;
using SSOSettingsFileManager;

namespace BtsPortal.Repositories.Helpers.Sso
{
    class BtdfManager
    {
        internal static List<SsoApplicationData> LoadBtdfSettingsFromSso(string appName)
        {
            var data = new List<SsoApplicationData>();

            Type settingsType = typeof(SSOSettingsManager);
            MethodInfo settingInfo = settingsType.GetMethod("GetSettings", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var settings = (Hashtable)settingInfo.Invoke(null, new object[] { appName, false });
            if (settings != null)
            {
                foreach (var key in settings.Keys)
                {
                    data.Add(new SsoApplicationData()
                    {
                        Key = key as string,
                        Value = settings[key] as string
                    });
                }
            }


            return data;

        }

        internal static void SaveBtdfSetting(string affiliateApplication, string propertyName, string propertyValue)
        {
            SSOSettingsManager.WriteSetting(affiliateApplication, propertyName, propertyValue);
        }
    }
}