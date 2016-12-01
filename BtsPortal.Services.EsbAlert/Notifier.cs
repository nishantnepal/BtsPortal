using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Xml;
using System.Xml.Xsl;
using BtsPortal.Cache;
using BtsPortal.Entities.Esb;
using BtsPortal.Repositories.Db;
using BtsPortal.Repositories.Interface;

namespace BtsPortal.Services.EsbAlert
{
    internal class Notifier : Worker
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
                    qInterval = Convert.ToInt32(esbConfiguration.FirstOrDefault(m => m.Name.ToUpper() == "EMAILINTERVAL")?.Value);
                }
                catch
                {
                    Process.HandleException("Invalid value of 'EMAILINTERVAL' in configuration settings. Using default of " + AppSettings.DEFAULT_INTERVAL, EventLogEntryType.Warning);
                }
                try
                {
                    qBatchSize = Convert.ToInt32(esbConfiguration.FirstOrDefault(m => m.Name.ToUpper() == "EMAILBATCHSIZE")?.Value);
                }
                catch
                {
                    Process.HandleException("Invalid value of 'EMAILBATCHSIZE' in configuration settings. Using default of " + AppSettings.DEFAULT_BATCH_SIZE, EventLogEntryType.Warning);
                }

                bool qEnabled = Convert.ToBoolean(esbConfiguration.FirstOrDefault(m => m.Name.ToUpper() == "ISEMAILENABLED")?.Value);
                string smtpServer = esbConfiguration.FirstOrDefault(m => m.Name.ToUpper() == "EMAILSERVER")?.Value;
                int? smtpPort = Convert.ToInt32(esbConfiguration.FirstOrDefault(m => m.Name.ToUpper() == "SMTPPORT")?.Value);
                string smtpUserName = esbConfiguration.FirstOrDefault(m => m.Name.ToUpper() == "SMTPCREDENTIALSUSERNAME")?.Value;
                string smtpPassword = esbConfiguration.FirstOrDefault(m => m.Name.ToUpper() == "SMTPCREDENTIALSPASSWORD")?.Value;
                bool smtpUseSsl = Convert.ToBoolean(esbConfiguration.FirstOrDefault(m => m.Name.ToUpper() == "SMTPENABLESSL")?.Value);
                string emailSender = esbConfiguration.FirstOrDefault(m => m.Name.ToUpper() == "SENDER")?.Value;
                string xsltPath = esbConfiguration.FirstOrDefault(m => m.Name.ToUpper() == "XSLTPATH")?.Value;
                string summaryXsltPath = esbConfiguration.FirstOrDefault(m => m.Name.ToUpper() == "XSLTSUMMARYPATH")?.Value;

                try
                {
                    if (qEnabled)
                    {
                        bool proceed = true;
                        if (!File.Exists(xsltPath))
                        {
                            System.Diagnostics.EventLog.WriteEntry(AppSettings.SERVICE_NAME, $"File '{xsltPath}' is not valid. Check configuration for XsltPath key", EventLogEntryType.Error);
                            proceed = false;
                        }

                        if (!File.Exists(summaryXsltPath))
                        {
                            System.Diagnostics.EventLog.WriteEntry(AppSettings.SERVICE_NAME, $"File '{summaryXsltPath}' is not valid. Check configuration for XsltPath key", EventLogEntryType.Error);
                            proceed = false;
                        }

                        if (proceed)
                        {
                            var data = _esbExceptionDbRepo.GetAlertEmails(qBatchSize);
                            ProcessNotifications(data, smtpServer, emailSender, xsltPath, summaryXsltPath, smtpPort, smtpUserName, smtpPassword, smtpUseSsl);
                            _esbExceptionDbRepo.UpdateAlertEmails(data);
                        }

                    }
                }
                catch (Exception exception)
                {
                    Process.HandleException(exception);
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

        private void ProcessNotifications(List<AlertEmailNotification> data, string smtpServer, string sender
            , string xsltPath, string summaryXsltPath, int? smptPort, string userName, string password, bool? enableSsl)
        {
            var transform = new XslCompiledTransform();
            XsltSettings settings = new XsltSettings { EnableScript = true };
            transform.Load(xsltPath, settings, new XmlUrlResolver());

            var summaryTransform = new XslCompiledTransform();
            summaryTransform.Load(summaryXsltPath, settings, new XmlUrlResolver());

            foreach (var notification in data)
            {
                try
                {
                    string mailBody = notification.IsSummaryAlert
                                        ? GetHtmlMailBody(notification, summaryTransform)
                                        : GetHtmlMailBody(notification, transform);
                    using (MailMessage message = new MailMessage(sender, notification.To, notification.Subject, mailBody))
                    {
                        message.IsBodyHtml = true;
                        SmtpClient emailClient = smptPort.HasValue ? new SmtpClient(smtpServer, smptPort.Value) : new SmtpClient(smtpServer);
                        if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
                        {
                            emailClient.Credentials = new NetworkCredential(userName, password);
                        }
                        if (enableSsl.HasValue)
                        {
                            emailClient.EnableSsl = enableSsl.Value;
                        }
                        emailClient.Send(message);
                        System.Diagnostics.Trace.WriteLine("Email successfully sent");

                    }
                }
                catch (Exception exception)
                {
                    notification.Error = exception.Message;
                }
                notification.Sent = true;
            }


        }

        private static string GetHtmlMailBody(AlertEmailNotification notification, XslCompiledTransform transform)
        {
            string mailBody;
            using (StringReader sr = new StringReader(notification.Body))
            {
                using (XmlReader xr = XmlReader.Create(sr))
                {
                    using (StringWriter sw = new StringWriter())
                    {
                        transform.Transform(xr, null, sw);
                        mailBody = sw.ToString();
                    }
                }
            }
            return mailBody;
        }
    }
}
