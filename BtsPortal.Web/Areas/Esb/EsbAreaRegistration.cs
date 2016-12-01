using System.Web.Mvc;

namespace BtsPortal.Web.Areas.Esb
{
    public class EsbAreaRegistration : AreaRegistration 
    {
        public override string AreaName => "Esb";

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Esb_default",
                "Esb/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}