using System;
using System.Collections.Generic;

namespace BtsPortal.Entities.Bts
{
    public class BtsSummary
    {
        public BtsSummary()
        {
            AppSummaries = new List<BtsAppSummary>();
            AppSummaryDates = new List<BtsAppSummaryDate>();
        }
        public List<BtsAppSummary> AppSummaries { get; set; }
        public List<BtsAppSummaryDate> AppSummaryDates{ get; set; }
    }

    public class BtsAppSummary
    {
        public int ApplicationId { get; set; }
        public string Application { get; set; }
        public int Running { get; set; }
        public int Dehydrated { get; set; }
        public int ReadyToRun { get; set; }
        public int SuspendedResumable { get; set; }
        public int SuspendedNonResumable { get; set; }
        public int OtherCount { get; set; }
    }


    public class BtsAppSummaryDate
    {
        public int ApplicationId { get; set; }
        public BtsInstanceStatus InstanceStatus { get; set; }
        public DateTime? MaxDate { get; set; }
        public DateTime? MinDate { get; set; }
    }


    public class BtsAppArtifactSummary
    {
        public string ApplicationName { get; set; }
        public int Application { get; set; }
        public Guid ServiceId { get; set; }
        public string ArtifactName { get; set; }
        public BtsArtifactType? ArtifactType { get; set; }
        public int InstancesCount { get; set; }
        public DateTime? DateCreatedMin { get; set; }
        public DateTime? DateCreatedMax { get; set; }
    }

    public class BtsModule
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
    }
    //public class BtsRunningInstance
    //{
    //    public string Application { get; set; }
    //    public DateTime CreatedDateTime { get; set; }
    //    public string Orchestration { get; set; }
    //    public string InstanceDetail { get; set; }
    //    public string ProcessingServer { get; set; }
    //    public string InstanceId { get; set; }
    //}
}
