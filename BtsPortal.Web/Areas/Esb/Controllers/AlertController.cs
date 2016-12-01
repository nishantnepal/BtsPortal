using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BtsPortal.Entities.Esb;
using BtsPortal.Repositories.Interface;
using BtsPortal.Web.Extensions;
using BtsPortal.Web.ViewModels;
using BtsPortal.Web.ViewModels.Esb;

namespace BtsPortal.Web.Areas.Esb.Controllers
{
    public class AlertController : Controller
    {
        private readonly IEsbExceptionDbRepository _esbExcRepository;
        private readonly IBizTalkRepository _btsRepository;
        public AlertController(IEsbExceptionDbRepository esbExceptionDbRepository, IBizTalkRepository bizTalkRepository)
        {
            _esbExcRepository = esbExceptionDbRepository;
            _btsRepository = bizTalkRepository;
        }
        public ActionResult Index()
        {
            var vm = _esbExcRepository.GetAlerts(User.Identity.Name);
            return View(vm);
        }

        public PartialViewResult CreateAlert()
        {
            var vm = new CreateAlert()
            {
                Applications = _btsRepository.GetBiztalkApplications().ToSelectListItems()
            };

            ViewBag.minutes = new List<int>() { 5, 15, 30, 45, 60, 90, 120, 150, 180, 210, 240, 300, 360 }.Select(min => new SelectListItem() { Text = min.ToString(), Value = min.ToString() }).ToList();
            return PartialView("_CreateAlert", vm);
        }

        public PartialViewResult EditAlert(Guid alertId)
        {
            var data = _esbExcRepository.GetAlert(alertId);
            ViewBag.applications = _btsRepository.GetBiztalkApplications().ToSelectListItems(selected: data.Alert.Name);
            ViewBag.minutes = new List<int>() { 5, 15, 30, 45, 60, 90, 120, 150, 180, 210, 240, 300, 360 }.Select(min => new SelectListItem()
            {
                Text = min.ToString(),
                Value = min.ToString(),
                Selected = data.Alert.IntervalMins.GetValueOrDefault(5) == min
            }).ToList();

            return PartialView("_EditAlert", data);
        }

        public JsonResult SaveAlert(string app, List<AlertCondition> conds, Guid? alertId, AlertType alertType, int? alertMins)
        {
            JsonResultWrapper wrapper = new JsonResultWrapper { Success = true };
            try
            {
                if (alertId.HasValue)
                {
                    UpdateAlert(alertId, alertType, alertMins);
                }
                else
                {
                    NewAlert(app, conds, alertType, alertMins);
                }

                wrapper.Data = "Successfully saved alert.";
            }
            catch (Exception exception)
            {
                wrapper.Success = false;
                wrapper.Error = exception.Message;
            }

            return Json(wrapper, JsonRequestBehavior.AllowGet);
        }

        private void UpdateAlert(Guid? alertId, AlertType alertType, int? alertMins)
        {
            _esbExcRepository.UpdateAlert(alertId.Value, alertType, alertMins);
        }

        private void NewAlert(string app, List<AlertCondition> conds, AlertType alertType, int? alertMins)
        {
            if (conds.Count == 0)
            {
                throw new Exception("Atleast one condition must be selected.");
            }
            string conditions = string.Join(" AND ", conds.Select(condition => condition.ToString()).ToList());

            if (alertType == AlertType.Transactional)
            {
                alertMins = null;
            }
            _esbExcRepository.SaveAlert(null, app, conditions, User.Identity.Name, alertType, alertMins, conds);
        }

        public JsonResult DeleteAlert(Guid? alertId)
        {
            JsonResultWrapper wrapper = new JsonResultWrapper { Success = true };
            try
            {
                if (alertId == null)
                {
                    throw new Exception("Invalid alert id.");
                }

                _esbExcRepository.DeleteAlert(alertId, User.Identity.Name);
                wrapper.Data = "Successfully deleted alert.";
            }
            catch (Exception exception)
            {
                wrapper.Success = false;
                wrapper.Error = exception.Message;
            }

            return Json(wrapper, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult AddSubscription(Guid alertId)
        {
            var vm = new EsbAlertSubscription()
            {
                AlertId = alertId,
                Subscriber = User.Identity.Name
            };
            return PartialView("_AddSubscription", vm);
        }

        [HttpPost]
        public JsonResult SaveSubscription(EsbAlertSubscription subscription)
        {
            JsonResultWrapper wrapper = new JsonResultWrapper { Success = true };
            try
            {
                _esbExcRepository.SaveAlertSubscription(subscription, User.Identity.Name);
                wrapper.Data = "Successfully saved subscription.";
            }
            catch (Exception exception)
            {
                wrapper.Success = false;
                wrapper.Error = exception.Message;
            }

            return Json(wrapper, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ToggleSubscriptionState(Guid alertSubscriptionId, bool currentState)
        {
            JsonResultWrapper wrapper = new JsonResultWrapper { Success = true };
            try
            {
                _esbExcRepository.ToggleAlertSubscriptionState(alertSubscriptionId, currentState, User.Identity.Name);
                wrapper.Data = "Successfully updated subscription state.";
            }
            catch (Exception exception)
            {
                wrapper.Success = false;
                wrapper.Error = exception.Message;
            }

            return Json(wrapper, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteSubscription(Guid alertSubscriptionId)
        {
            JsonResultWrapper wrapper = new JsonResultWrapper { Success = true };
            try
            {
                _esbExcRepository.DeleteAlertSubscription(alertSubscriptionId);
                wrapper.Data = "Successfully deleted subscription.";
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