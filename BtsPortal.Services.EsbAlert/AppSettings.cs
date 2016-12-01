﻿using System.Configuration;

namespace BtsPortal.Services.EsbAlert
{
    internal class AppSettings
    {
        public static string EsbExceptionDbConnectionString
            => ConfigurationManager.ConnectionStrings["EsbExceptionDbConnection"].ConnectionString;

        public const string SERVICE_NAME = "IA.ESB.Alert";
        public const int DEFAULT_INTERVAL = 60000;
        public const int DEFAULT_BATCH_SIZE = 500;
    }
}
