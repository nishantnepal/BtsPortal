using System.Globalization;
using Microsoft.Win32;

namespace BtsPortal.Entities.Bts
{
    internal class BtsManagementDatabaseConnection
    {
        static BtsManagementDatabaseConnection()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\BizTalk Server\3.0\Administration");
            ServerName = (string)key.GetValue("MgmtDBServer");
            DatabaseName = (string)key.GetValue("MgmtDBName");
            key.Close();
        }

        public static string ServerName { get; }

        public static string DatabaseName { get; }

        public static string ConnectionString
        {
            get
            {
                string format = "Server={0};Database={1};Integrated Security=SSPI";
                return string.Format(CultureInfo.CurrentCulture, format, new object[] { ServerName, DatabaseName });
            }

        }
    }
}
