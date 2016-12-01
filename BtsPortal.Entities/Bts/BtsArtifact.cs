using System;

namespace BtsPortal.Entities.Bts
{
    public class BtsArtifact
    {
        public BtsArtifactType ArtifactType { get; set; }
        public string ArtifactName { get; set; }
        public Guid ServiceId { get; set; }
        public string ApplicationName { get; set; }
    }
}