using System.Collections.Generic;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using BtsPortal.Entities.Esb;
using PagedList;

namespace BtsPortal.Web.ViewModels.Esb
{
    public class FaultSearchRequestVm : FaultSearchRequest
    {
        public bool Init { get; set; }
        public List<SelectListItem> Applications { get; set; }
    }

    public class FaultSearchRequestResponse
    {
        public FaultSearchRequestResponse()
        {
            Responses = new ListStack<FaultSearchResponse>();
        }
        public FaultSearchRequestVm Request { get; set; }

        public FaultStatusType? CurrentStatus => Request?.Status;

        public List<FaultSearchResponse> Responses { get; set; }
        public int? PageSize { get; set; }
        public int? Page { get; set; }
        public int? TotalRows { get; set; }

        public string Error { get; set; }
        public IPagedList<FaultSearchResponse> PagedResponses
        {
            get
            {
                return new StaticPagedList<FaultSearchResponse>(Responses, Page.GetValueOrDefault(1), PageSize.GetValueOrDefault(1), TotalRows.GetValueOrDefault(0));
            }
        }
    }
}