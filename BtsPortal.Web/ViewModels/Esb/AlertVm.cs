using System.Collections.Generic;
using System.Web.Mvc;
using BtsPortal.Entities.Esb;

namespace BtsPortal.Web.ViewModels.Esb
{
    public class CreateAlert
    {
        public string Application { get; set; }
        public string ConditionsString { get; set; }
        public List<SelectListItem> Applications { get; set; }
        public List<string> AlertCriteria { get; set; }
        public AlertType AlertType { get; set; }
        public int SummaryIntervalMinutes { get; set; }
    }

    
}