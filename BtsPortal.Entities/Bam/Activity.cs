using System.Xml.Linq;

namespace BtsPortal.Entities.Bam
{
    public class Activity
    {
        public string ActivityName { get; set; }
        public XElement DefinitionXml { get; set; }
    }
}
