using System;
using BtsPortal.Entities.Bts;
using Dapper;
using Microsoft.BizTalk.B2B.PartnerManagement;
using Microsoft.BizTalk.ExplorerOM;

namespace BtsPortal.Repositories.Utilities
{
    internal static class Extensions
    {
        public static DbString ToDbStringAnsi(this string value, int length = 100)
        {
            return new DbString { Value = value, Length = length, IsAnsi = true };

        }

        public static BatchFilterOperator ToBatchFilterOperator(this FilterOperator fOp)
        {
            switch (fOp)
            {
                case FilterOperator.Equals: return BatchFilterOperator.Equals;
                //todo
                default: return BatchFilterOperator.Equals;
            }
        }

        public static BtsEdiRecurrenceType ToBtsEdiRecurrenceType(this RecurrenceSchedule rs)
        {
            if (rs.GetType() == typeof(HourlyRecurrence))
            {
                return BtsEdiRecurrenceType.Hourly;
            }
            if (rs.GetType() == typeof(DailyRecurrence))
            {
                return BtsEdiRecurrenceType.Daily;
            }
            if (rs.GetType() == typeof(WeeklyRecurrence))
            {
                return BtsEdiRecurrenceType.Weekly;
            }

            return BtsEdiRecurrenceType.None;
        }

        public static BtsArtifactStatus ToBtsArtifactStatus(this PortStatus portStatus)
        {
            switch (portStatus)
            {
                case PortStatus.Bound:
                    return BtsArtifactStatus.Bound;
                case PortStatus.Stopped:
                    return BtsArtifactStatus.Stopped;
                case PortStatus.Started:
                    return BtsArtifactStatus.Started;
                default:
                    throw new Exception("Unexpected Port Status : " + portStatus.ToString());
            }
        }

        public static PortStatus ToBtsArtifactStatus(this BtsArtifactStatus portStatus)
        {
            switch (portStatus)
            {
                case BtsArtifactStatus.Bound:
                    return PortStatus.Bound;
                case BtsArtifactStatus.Stopped:
                    return PortStatus.Stopped;
                case BtsArtifactStatus.Started:
                    return PortStatus.Started;
                default:
                    throw new Exception("Unexpected Port Status : " + portStatus.ToString());
            }
        }

        public static BtsArtifactStatus ToBtsArtifactStatus(this OrchestrationStatus portStatus)
        {
            switch (portStatus)
            {
                case OrchestrationStatus.Unenlisted:
                    return BtsArtifactStatus.Unenlisted;
                case OrchestrationStatus.Enlisted:
                    return BtsArtifactStatus.Enlisted;
                case OrchestrationStatus.Started:
                    return BtsArtifactStatus.Started;
                default:
                    throw new Exception("Unexpected Port Status : " + portStatus.ToString());
            }
        }

        public static OrchestrationStatus ToOrchestrationStatus(this BtsArtifactStatus portStatus)
        {
            switch (portStatus)
            {
                case BtsArtifactStatus.Unenlisted:
                    return OrchestrationStatus.Unenlisted;
                case BtsArtifactStatus.Enlisted:
                    return OrchestrationStatus.Enlisted;
                case BtsArtifactStatus.Started:
                    return OrchestrationStatus.Started;
                default:
                    throw new Exception("Unexpected Port Status : " + portStatus.ToString());
            }
        }

    }
}
