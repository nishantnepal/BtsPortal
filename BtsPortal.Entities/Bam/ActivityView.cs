using System;
using System.Collections.Generic;

namespace BtsPortal.Entities.Bam
{
    public class ActivityView
    {
        public ActivityView()
        {
            ViewFilterParameters = new List<ActivityViewFilterParameter>();
        }
        public int? Id { get; set; }
        public int? NoOfRowsPerPage { get; set; }
        public bool IsActive { get; set; }
        public string ActivityName { get; set; }
        public string ViewName { get; set; }
        public string SqlToExecute { get; set; }
        public string SqlOrderBy { get; set; }
        public string SqlNoOfRows { get; set; }
        public string InsertedBy { get; set; }
        public DateTime? InsertedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public string FilterParameters { get; set; }
        public List<ActivityViewFilterParameter> ViewFilterParameters { get; set; }
    }

    public class ActivityViewFilterParameter
    {
        public int? Id { get; set; }
        public int BamActivityViewId { get; set; }
        public string Name { get; set; }
        public string Operator { get; set; }
        public string DisplayName { get; set; }
        public ParameterType ParameterType { get; set; }
        public string Value { get; set; }

        //public override string ToString()
        //{
        //    return $"{Criteria} {Operation} '{Value}'";
        //}
    }

    public enum ParameterType
    {
        Character = 1,
        Number,
        Date
    }
}
