using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BtsPortal.Cache;
using BtsPortal.Entities.Bam;
using BtsPortal.Repositories.Interface;
using BtsPortal.Web.Controllers;
using BtsPortal.Web.Infrastructure.Settings;
using BtsPortal.Web.ViewModels;

namespace BtsPortal.Web.Areas.Bam.Controllers
{
    public class ActivityController : BaseController
    {
        private readonly IBamRepository _bamRepository;
        private readonly IBtsPortalRepository _btsPortalRepository;
        private readonly ICacheProvider _cacheProvider;
        public ActivityController(IBamRepository bamRepository, IBtsPortalRepository btsPortalRepository, ICacheProvider cacheProvider)
        {
            _bamRepository = bamRepository;
            _btsPortalRepository = btsPortalRepository;
            _cacheProvider = cacheProvider;
        }
        // GET: Bam/Activity
        public ActionResult Index(string activityName, string viewName)
        {
            if (!_bamRepository.BamInstalled())
            {
                ViewBag.BamNotInstalled =
                    "Either BAM has not been configured or the connection string to the BAMPrimaryImport database is wrong.";

                return View();
            }

            var data = _bamRepository.LoadActivities();
            var activities = data.Select(m => m.ActivityName).ToList().Select(activity => new SelectListItem()
            {
                Value = activity,
                Text = activity,
                Selected = string.Equals(activity, activityName, StringComparison.OrdinalIgnoreCase)
            }).ToList();
            activities.Insert(0, new SelectListItem() { Text = "--Select Activity--", Value = String.Empty });

            if (!string.IsNullOrEmpty(viewName))
            {
                var activityViews = _btsPortalRepository.LoadActivitiesViews();
                var views = (from item in activityViews
                             where string.Equals(activityName, item.ActivityName, StringComparison.OrdinalIgnoreCase)
                             select new SelectListItem()
                             {
                                 Text = item.ViewName,
                                 Value = item.Id.ToString(),
                                 Selected = string.Equals(item.ViewName, viewName, StringComparison.OrdinalIgnoreCase)

                             }).ToList();
                activities.Insert(0, new SelectListItem() { Text = "--Select View--", Value = String.Empty });
                ViewBag.views = views;

            }

            ViewBag.activities = activities;
            return View();
        }
        public JsonResult GetActivityView(string activityName)
        {
            var vm = new List<JsonKeyValue>();
            if (string.IsNullOrWhiteSpace(activityName))
            {
                return Json(vm, JsonRequestBehavior.AllowGet);
            }
            var activities = _btsPortalRepository.LoadActivitiesViews(true);
            vm.AddRange(from item in activities
                        where string.Equals(activityName, item.ActivityName, StringComparison.OrdinalIgnoreCase)
                        select new JsonKeyValue()
                        {
                            Key = item.ViewName,
                            Value = item.Id.ToString()
                        });

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult LoadActivity(int? viewId, int? page, List<JsonKeyValue> filterData)
        {
            var bamData = new BamResult();
            if (!viewId.HasValue)
            {
                return PartialView("_LoadActivity", bamData);
            }
            try
            {
                var vw = _btsPortalRepository.LoadActivityView(viewId.Value);
                bamData.FilterParameters = vw.ViewFilterParameters;
                bamData.ViewId = viewId.GetValueOrDefault(0);

                if (vw.ViewFilterParameters.Count == 0)
                {
                    bamData = _bamRepository.LoadData(vw, page.GetValueOrDefault(1), AppSettings.PageSize, null);
                    bamData.FilterParameters = vw.ViewFilterParameters;
                }
                else if (filterData != null && filterData.Count > 0)
                {
                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    foreach (var keyValue in filterData)
                    {
                        if (!parameters.ContainsKey(keyValue.Key))
                        {
                            parameters.Add(keyValue.Key, keyValue.Value);
                        }
                    }

                    bamData = _bamRepository.LoadData(vw, page.GetValueOrDefault(1), AppSettings.PageSize, parameters);
                    bamData.FilterParameters = vw.ViewFilterParameters;
                    if (bamData.FilterParameters != null)
                    {
                        foreach (var filterParameter in bamData.FilterParameters)
                        {
                            if (parameters.ContainsKey(filterParameter.Name))
                            {
                                filterParameter.Value = parameters[filterParameter.Name];
                            }
                        }
                    }

                }

            }
            catch (Exception exception)
            {
                bamData.ViewId = viewId.GetValueOrDefault(0);
                ViewBag.error =
                    exception.Message.ToUpper().Contains("INVALID USAGE OF THE OPTION NEXT IN THE FETCH STATEMENT")
                        ? exception.Message + Environment.NewLine +
                          "[Check the sql configured to be executed to ensure that it doesnt end with order by]"
                        : exception.Message;
            }


            return PartialView("_LoadActivity", bamData);

        }


    }
}