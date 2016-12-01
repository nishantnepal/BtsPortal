using System;
using System.Collections.Generic;

namespace BtsPortal.Entities.Esb
{
    public class AlertNotification
    {
        public Guid BatchId { get; set; }
        public List<FaultDetail> FaultDetails { get; set; }
        public List<AlertFault> AlertFaults { get; set; }
        public List<EsbAlert> Alerts { get; set; }
        public List<EsbAlertSubscription> AlertSubscriptions { get; set; }
        public List<AlertFaultSummary> AlertFaultSummaries { get; set; }
    }

    public class AlertFault
    {
        public Guid AlertId { get; set; }
        public Guid FaultId { get; set; }
    }

    public class AlertEmailNotification
    {
        public Guid AlertEmailId { get; set; }
        public Guid AlertId { get; set; }
        public string Body { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public Guid BatchId { get; set; }
        public string Error { get; set; }
        public bool Sent { get; set; }
        public bool IsSummaryAlert { get; set; }
    }

    public class AlertFaultSummary
    {
        public Guid AlertId { get; set; }
        public string Application { get; set; }
        public string ExceptionType { get; set; }
        public string ServiceName { get; set; }
        public int Count { get; set; }
        public DateTime MinTime { get; set; }
        public DateTime MaxTime { get; set; }
       
    }
}
