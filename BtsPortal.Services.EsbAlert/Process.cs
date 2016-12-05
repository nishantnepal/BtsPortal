using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using BtsPortal.Cache;
using BtsPortal.Entities.Esb;
using BtsPortal.Repositories.Db;

namespace BtsPortal.Services.EsbAlert
{
    class Process
    {
        //readonly Timer _timer;
       private readonly ICacheProvider _cacheProvider;
        private List<EsbConfiguration> _esbConfigurations;
        private Worker _queueGen;
        private Worker _notifier;
        private Worker _summaryQueGen;
        private Thread _notifierThread;
        private Thread _aggregratorThread;
        private Thread _aggregrator2Thread;

        public Process()
        {
            //_timer = new Timer(1000) { AutoReset = true };
            //_timer.Elapsed += (sender, eventArgs) => Console.WriteLine("It is {0} and all is well", DateTime.Now);
           _cacheProvider = new MemoryCacheProvider();
        }

        public void Start()
        {
            TestAndLoadSettings();
            StartProcess();
            //todo : fire off queuegenerator and Notifier
            //_timer.Start();
        }

        private void StartProcess()
        {
            _queueGen = new QueueGenerator();
            _aggregratorThread = new Thread(() => _queueGen.DoWork(_cacheProvider));
            _aggregratorThread.Start();

            _summaryQueGen = new SummaryQueueGenerator();
            _aggregrator2Thread = new Thread(() => _summaryQueGen.DoWork(_cacheProvider));
            _aggregrator2Thread.Start();

            //Start the thread to read the alerts from the email queue and send notification emails.
            _notifier = new Notifier();
            _notifierThread = new Thread(() => _notifier.DoWork(_cacheProvider));
            _notifierThread.Start();
        }

        private void TestAndLoadSettings()
        {
            TestDbConnection();
            LoadEsbConfiguration();
            TestLdapConnection();
            TestEventLog();

        }

        private void TestEventLog()
        {
           EventLog.WriteEntry(AppSettings.SERVICE_NAME,"Settings tested successfully on startup");
        }

        private void LoadEsbConfiguration()
        {
            var esbExceptionDbRepository = new EsbExceptionDbRepository(new SqlConnection(AppSettings.EsbExceptionDbConnectionString));
            _esbConfigurations = esbExceptionDbRepository.GetEsbConfiguration();
        }

        private void TestLdapConnection()
        {
            string ldapRoot= string.Empty;
            try
            {
                ldapRoot = _esbConfigurations.FirstOrDefault(m => m.Name.ToUpper() == "LDAPROOT")?.Value;
                ActiveDirectoryHelper.TestLDAPConnectivity(ldapRoot);
            }
            catch (Exception)
            {
                throw new Exception(
                    $"Unable to confirm ldap settings '{ldapRoot}'.");
            }
        }

        private static void TestDbConnection()
        {
            try
            {
                using (var conn = new SqlConnection(AppSettings.EsbExceptionDbConnectionString))
                {
                    conn.Open();
                    conn.Close();
                }
            }
            catch (Exception)
            {
                throw new Exception(
                    $"Unable to connect to esb exception database '{AppSettings.EsbExceptionDbConnectionString}'.");
            }


        }

        internal static void HandleException(Exception ex)
        {
            var preColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ForegroundColor = preColor;

            EventLog.WriteEntry(AppSettings.SERVICE_NAME, ex.ToString(), EventLogEntryType.Error);
        }

        internal static void HandleException(string msg, EventLogEntryType entryType = EventLogEntryType.Error)
        {
            var preColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ForegroundColor = preColor;

            EventLog.WriteEntry(AppSettings.SERVICE_NAME, msg, entryType);
        }

        public void Stop()
        {
            EventLog.WriteEntry(AppSettings.SERVICE_NAME,"Stopping queue generator.");
            //_timer.Stop();
            // Try stopping Notifier
            if (_queueGen != null &&
                _queueGen.CanForceStop &&
                _aggregratorThread != null)
            {
                try
                {
                    _aggregratorThread.Abort();
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }

            }
            else
            {
                if (_queueGen != null) _queueGen.Stop();
            }

            EventLog.WriteEntry(AppSettings.SERVICE_NAME, "Stopping summary queue generator.");
            if (_summaryQueGen != null &&
                _summaryQueGen.CanForceStop &&
                _aggregrator2Thread != null)
            {
                try
                {
                    _aggregrator2Thread.Abort();
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }

            }
            else
            {
                if (_summaryQueGen != null) _summaryQueGen.Stop();
            }

            EventLog.WriteEntry(AppSettings.SERVICE_NAME, "Stopping notofier.");
            if (_notifier != null &&
                _notifier.CanForceStop &&
                _notifierThread != null)
            {
                try
                {
                    _notifierThread.Abort();
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }

            }
            else
            {
                if (_notifier != null) _notifier.Stop();
            }
        }


    }
}
