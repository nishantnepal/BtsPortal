using System;

namespace BtsPortal.Entities.Bts
{
    public class BtsInstance
    {
        public string Application { get; set; }
        public int ApplicationId { get; set; }
        public string ArtifactName { get; set; }
        public BtsArtifactType? ArtifactType { get; set; }
        public BtsInstanceStatus InstanceStatus { get; set; }
        public Guid ServiceId { get; set; }
        public Guid InstanceId { get; set; }
        public DateTime DateCreated { get; set; }

        public DateTime DateCreatedLocal => DateCreated.ToLocalTime();

        public DateTime? DateSuspended { get; set; }

        public string ProcessingServer { get; set; }
        public string ErrorDescription { get; set; }
    }
}