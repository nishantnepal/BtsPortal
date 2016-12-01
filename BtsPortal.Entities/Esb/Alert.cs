using System;
using System.Collections.Generic;
using System.Text;

namespace BtsPortal.Entities.Esb
{
    public class EsbUserAlertSubscription
    {
        public string CurrentUser { get; set; }
        public List<EsbAlert> Alerts { get; set; }
        public List<EsbAlertSubscription> MyAlertSubscriptions { get; set; }

    }

    public class EsbAlert
    {
        public EsbAlert()
        {
            AlertSubscriptions = new List<EsbAlertSubscription>();
            _builder = new StringBuilder();
        }

        private StringBuilder _builder;
        public Guid AlertId { get; set; }
        public string Name { get; set; }
        public string ConditionsString { get; set; }
        public string InsertedBy { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime? LastFired { get; set; }
        public int TotalSubscriberCount { get; set; }
        public int? IntervalMins { get; set; }
        public bool IsSummaryAlert { get; set; }

        public AlertType AlertType
        {
            get
            {
                if (IsSummaryAlert)
                {
                    return AlertType.Summary;;
                }
                else
                {
                    return AlertType.Transactional;
                }
            }
        }

        public List<EsbAlertSubscription> AlertSubscriptions { get; set; }

        public string Subscribers
        {
            get
            {
                if (_builder.Length != 0) return _builder.ToString();
                foreach (var subscription in AlertSubscriptions)
                {
                    _builder.AppendLine(string.Concat(subscription.Subscriber, " - ", subscription.CustomEmail));
                }

                return _builder.ToString();
            }
        }


    }

    public class EsbAlertSubscription
    {
        public Guid AlertId { get; set; }
        public Guid AlertSubscriptionId { get; set; }
        public string Name { get; set; }
        public string ConditionsString { get; set; }
        public string Subscriber { get; set; }
        public string CustomEmail { get; set; }
        public string InsertedBy { get; set; }
        public bool IsGroup { get; set; }
        public DateTime InsertedDate { get; set; }
        public bool Active { get; set; }
        //public DateTime? LastFired { get; set; }
        //public int IntervalMinutes { get; set; }
    }

    public class AlertCondition
    {
        public string Criteria { get; set; }
        public string Operation { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Criteria} {Operation} '{Value}'";
        }
    }

    public class AlertView
    {
        public EsbAlert Alert { get; set; }
        public List<AlertCondition> AlertConditions { get; set; }
    }

}