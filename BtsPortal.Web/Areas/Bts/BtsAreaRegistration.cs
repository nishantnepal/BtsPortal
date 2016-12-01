using System.Web.Http;
using System.Web.Mvc;

namespace BtsPortal.Web.Areas.Bts
{
    public class BtsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Bts";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Bts_default",
                "Bts/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            context.Routes.MapHttpRoute("Bts_default_api",
                "Bts/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
                );


        }
    }
}