using System.Globalization;

namespace BtsPortal.Entities.Bts
{
    internal class BtsMsgBoxDatabaseConnection
    {
        //static BtsMsgBoxDatabaseConnection()
        //{
        //    LoadMessageBoxConnectionString();
        //}

        //static void LoadMessageBoxConnectionString()
        //{
        //    using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("ROOT\\MicrosoftBizTalkServer", "Select * from MSBTS_GroupSetting"))
        //    {
        //        foreach (ManagementObject managementObject in managementObjectSearcher.Get())
        //        {
        //            DatabaseName = (string)managementObject["SubscriptionDBName"];
        //            ServerName = (string)managementObject["SubscriptionDBServerName"];
        //        }
        //    }
        //}

        //public static string ServerName { get; private set; }

        //public static string DatabaseName { get; private set; }

        //public static string ConnectionString
        //{
        //    get
        //    {
        //        string format = "Server={0};Database={1};Integrated Security=SSPI";
        //        return string.Format(CultureInfo.CurrentCulture, format, new object[] { ServerName, DatabaseName });
        //    }

        //}
    }
}
