using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BtsPortal.Entities.Bts;
using PagedList;

namespace BtsPortal.Web.ViewModels.Bts
{
    public class BtsSearchRequestResponse
    {
        public BtsSearchRequestResponse()
        {
            Responses = new List<BtsInstance>();
        }
        public BtsSearchRequest Request { get; set; }
        public List<BtsInstance> Responses { get; set; }
        public int? PageSize { get; set; }
        public int? Page { get; set; }
        public int? TotalRows { get; set; }
        public string Error { get; set; }
        public IPagedList<BtsInstance> PagedResponses
        {
            get
            {
                return new StaticPagedList<BtsInstance>(Responses, Page.GetValueOrDefault(1), PageSize.GetValueOrDefault(1), TotalRows.GetValueOrDefault(0));
            }
        }
    }

    public class BtsSearchRequest
    {
        public BtsSearchRequest()
        {
            ArtifactIds = new List<SelectListItem>();
            Applications = new List<SelectListItem>();
        }
        public bool Init { get; set; }
        public BtsArtifactType? ArtifactType { get; set; }
        public Guid? ArtifactId { get; set; }
        public List<SelectListItem> ArtifactIds { get; set; }
        public BtsInstanceStatus Status { get; set; }
        public int Application { get; set; }
        public List<SelectListItem> Applications { get; set; }
        public DateTime? DateCreatedFrom { get; set; }
        public DateTime? DateCreatedTo { get; set; }
    }

    
}