using System;
using System.Collections.Generic;

namespace BtsPortal.Entities.Esb
{
    public class FaultSummary
    {
        public string Application { get; set; }
        public string ErrorType { get; set; }
        public int UnResolved { get; set; }
        public int Resubmitted { get; set; }
        public int Resolved { get; set; }
        public int Flagged { get; set; }
        public DateTime? LastFaultOccurredDateUtc { get; set; }

        public DateTime? LastFaultOccurredDateLocal => LastFaultOccurredDateUtc.GetValueOrDefault().ToLocalTime();
    }

    public class FaultSummaryVm
    {
        public FaultSummaryVm()
        {
            FaultSummariesByApplication = new List<FaultSummary>();
            FaultSummariesByErrorType = new List<FaultSummary>();
        }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<FaultSummary> FaultSummariesByApplication { get; set; }
        public List<FaultSummary> FaultSummariesByErrorType { get; set; }
    }
}