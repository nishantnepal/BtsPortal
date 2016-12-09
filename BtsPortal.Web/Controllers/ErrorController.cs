using System.Web.Mvc;

namespace BtsPortal.Web.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AccessDenied(string roles, string errorDesc)
        {
            ViewBag.Roles = roles;
            ViewBag.error = errorDesc;
            return View();
        }

        public PartialViewResult AccessDeniedPartial(string roles, string errorDesc)
        {
            ViewBag.Roles = roles;
            ViewBag.error = errorDesc;
            return PartialView("_AccessDenied");
        }
    }
}