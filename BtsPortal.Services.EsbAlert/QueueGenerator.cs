using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using BtsPortal.Cache;
using BtsPortal.Entities.Esb;
using BtsPortal.Helpers;
using BtsPortal.Repositories.Db;
using BtsPortal.Repositories.Interface;

namespace BtsPortal.Services.EsbAlert
{
    internal class QueueGenerator : Worker
    {
        private IEsbExceptionDbRepository _esbExceptionDbRepo;
        private ICacheProvider _cacheProvider;
        string LdapRoot { get; set; }

        internal override void DoWork(ICacheProvider cacheProvider)
        {
            CanForceStop = false;
            while (!MarkAsStop)
            {
                _esbExceptionDbRepo = new EsbExceptionDbRepository(new SqlConnection(AppSettings.EsbExceptionDbConnectionString));
                _cacheProvider = cacheProvider;

                var esbConfiguration = _esbExceptionDbRepo.GetEsbConfiguration();
                int qInterval = AppSettings.DEFAULT_INTERVAL;
                int qBatchSize = AppSettings.DEFAULT_BATCH_SIZE;
                try
                {
                    qInterval = Convert.ToInt32(esbConfiguration.FirstOrDefault(m => m.Name.ToUpper() == "QUEUEINTERVAL")?.Value);
                }
                catch
                {
                    Process.HandleException("Invalid value of 'QUEUEINTERVAL' in configuration settings. Using default of " + AppSettings.DEFAULT_INTERVAL, EventLogEntryType.Warning);
                }
                bool qEnabled = Convert.ToBoolean(esbConfiguration.FirstOrDefault(m => m.Name.ToUpper() == "ISQUEUEENABLED")?.Value);
                try
                {
                    qBatchSize = Convert.ToInt32(esbConfiguration.FirstOrDefault(m => m.Name.ToUpper() == "QUEUEBATCHSIZE")?.Value);
                }
                catch
                {
                    Process.HandleException("Invalid value of 'QueueBatchSize' in configuration settings. Using default of " + AppSettings.DEFAULT_BATCH_SIZE, EventLogEntryType.Warning);
                }

                LdapRoot = esbConfiguration.FirstOrDefault(m => m.Name.ToUpper() == "LDAPROOT")?.Value;


                try
                {
                    if (qEnabled)
                    {
                        //load unprocessed faults
                        var data = _esbExceptionDbRepo.GetAlertNotifications(qBatchSize);

                        try
                        {
                            //process the faults
                            ProcessFaults(data);
                            _esbExceptionDbRepo.UpdateAlertBatchComplete(data.BatchId, null);
                        }
                        catch (Exception exception)
                        {
                            _esbExceptionDbRepo.UpdateAlertBatchComplete(data.BatchId, exception.Message);
                        }
                    }
                }
                finally
                {
                    CanForceStop = true;
                    try
                    {
                        if (!MarkAsStop)
                        {
                            //Sleep
                            System.Threading.Thread.Sleep(qInterval);
                        }
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        //Sleep five seconds
                        System.Threading.Thread.Sleep(5000);
                    }
                }
            }
        }

        private void ProcessFaults(AlertNotification data)
        {
            List<Guid> alerts = data.AlertFaults.Select(m => m.AlertId).Distinct().ToList();
            List<AlertEmailNotification> notifications = new List<AlertEmailNotification>();
            foreach (var alertId in alerts)
            {
                var alertFaultIds = data.AlertFaults.Where(m => m.AlertId == alertId).Select(m => m.FaultId).Distinct().ToList();
                var alertFaults = data.FaultDetails.Where(m => alertFaultIds.Contains(m.FaultId)).ToList();
                var alertSubscriptions = data.AlertSubscriptions.Where(m => m.AlertId == alertId).ToList();
                var alert = data.Alerts.FirstOrDefault(m => m.AlertId == alertId);

                //if (DateTime.Now.Subtract(alert.LastFired.GetValueOrDefault(DateTime.MinValue)).Minutes >= alert.IntervalMins.GetValueOrDefault(0))
                //{
                List<string> emails = GetEmails(alertSubscriptions);
                string xml = SerializationHelper.Serialize(alertFaults);
                notifications.Add(new AlertEmailNotification()
                {
                    AlertId = alertId,
                    BatchId = data.BatchId,
                    Body = xml,
                    To = string.Join(",", emails),
                    Subject = string.Format("Esb Fault - {0}", alert.Name),
                    IsSummaryAlert = false
                });
                //}
            }

            _esbExceptionDbRepo.InsertAlertEmail(notifications);
            _esbExceptionDbRepo.UpdateAlertLastFired(alerts, DateTime.Now);

        }

        private List<string> GetEmails(List<EsbAlertSubscription> alertSubscriptions)
        {
            List<string> emails = new List<string>();
            foreach (var subscription in alertSubscriptions)
            {
                if (subscription.IsGroup)
                {
                    foreach (
                        SearchResult result in
                            ActiveDirectoryHelper.GetMembersOfGroup(subscription.Subscriber, _cacheProvider, LdapRoot))
                    {
                        //System.Diagnostics.Trace.WriteLine("Email notification added for fault " + fault.FaultID.ToString() + " which has InsertedDate as " + fault.InsertedDate.ToString());
                        string email = Convert.ToString(result.Properties["mail"][0]);
                        if (!emails.Contains(email))
                        {
                            emails.Add(email);
                        }
                    }
                }
                else
                {
                    //System.Diagnostics.Trace.WriteLine("Email notification added for fault " + fault.FaultID.ToString() + " which has InsertedDate as " + fault.InsertedDate.ToString());
                    string email = string.IsNullOrEmpty(subscription.CustomEmail)
                        ? ActiveDirectoryHelper.GetEmailAddress(subscription.Subscriber, _cacheProvider, LdapRoot)
                        : subscription.CustomEmail;
                    if (!emails.Contains(email) && !string.IsNullOrWhiteSpace(email))
                    {
                        emails.Add(email);
                    }
                }
            }

            return emails;
        }
    }



}
