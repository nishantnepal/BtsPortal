using System.Collections.Generic;
using System.Data;

namespace BtsPortal.Entities.Bam
{
    public class BamResult
    {
        public BamResult()
        {
            FilterParameters = new List<ActivityViewFilterParameter>();
            ResultDataTable = new DataTable();
        }

        public int? PageNumber { get; set; }
        public int ViewId { get; set; }
        public string ViewName { get; set; }
        public string ActivityName { get; set; }
        public int? PageSize { get; set; }
        public int? TotalPage { get; set; }
        public int? TotalRows { get; set; }
        public DataTable ResultDataTable { get; set; }
        public List<ActivityViewFilterParameter> FilterParameters { get; set; }
    }
}
