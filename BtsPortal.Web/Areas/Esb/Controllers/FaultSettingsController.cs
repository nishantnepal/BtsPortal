using System;
using System.Web.Mvc;
using BtsPortal.Web.Repository;
using BtsPortal.Web.ViewModels;

namespace BtsPortal.Web.Areas.Esb.Controllers
{
    public class FaultSettingsController : Controller
    {
        private readonly CachedEsbExceptionDbRepository _esbExcRepository;
        public FaultSettingsController()
        {
            _esbExcRepository = new CachedEsbExceptionDbRepository();
        }

        // GET: Esb/FaultSettings
        public ActionResult Index()
        {
            var vm = _esbExcRepository.GetEsbConfiguration();
            return View(vm);
        }

        public JsonResult UpdateEsbSetting(Guid configurationId, string value)
        {
            JsonResultWrapper wrapper = new JsonResultWrapper { Success = true };
            try
            {
                _esbExcRepository.UpdateEsbConfiguration(configurationId, value, User.Identity.Name);
                wrapper.Data = "Success.";
            }
            catch (Exception exception)
            {
                wrapper.Success = false;
                wrapper.Error = exception.Message;
            }

            return Json(wrapper, JsonRequestBehavior.AllowGet);
        }
    }
}