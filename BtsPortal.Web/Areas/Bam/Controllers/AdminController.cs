using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BtsPortal.Cache;
using BtsPortal.Entities.Bam;
using BtsPortal.Repositories.Interface;
using BtsPortal.Web.Infrastructure.Settings;
using BtsPortal.Web.ViewModels;

namespace BtsPortal.Web.Areas.Bam.Controllers
{
    public class AdminController : Controller
    {
        private readonly IBamRepository _bamRepository;
        private readonly IBtsPortalRepository _btsPortalRepository;
        private readonly ICacheProvider _cacheProvider;
        public AdminController(IBamRepository bamRepository, IBtsPortalRepository btsPortalRepository, ICacheProvider cacheProvider)
        {
            _bamRepository = bamRepository;
            _btsPortalRepository = btsPortalRepository;
            _cacheProvider = cacheProvider;
        }
        // GET: Bam/Admin
        public ActionResult Index()
        {
            var data = _bamRepository.LoadActivities();
            var activities = data.Select(m => m.ActivityName).ToList().Select(activity => new SelectListItem()
            {
                Value = activity,
                Text = activity,
            }).ToList();
            activities.Insert(0, new SelectListItem() { Text = "--Select Activity--", Value = String.Empty });

            ViewBag.activities = activities;
            return View();
        }

        public PartialViewResult ActivityView(string activityName)
        {
            var activityViews = _btsPortalRepository.LoadActivitiesViews()
                                .Where(m => string.Equals(m.ActivityName, activityName, StringComparison.OrdinalIgnoreCase)).ToList();
            return PartialView("_ActivityView", activityViews);
        }

        public PartialViewResult Configuration(int? id, string activityName)
        {
            var vm = new ActivityView();

            var typeList = (from ParameterType entity in Enum.GetValues(typeof(ParameterType))
                            select new SelectListItem()
                            {
                                Text = entity.ToString(),
                                Value = entity.ToString()
                            }).ToList();

            ViewBag.typeList = typeList;
            var pageSizes = new List<SelectListItem>();

            if (!id.HasValue)
            {
                vm.ActivityName = activityName;
                vm.Id = 0;
                ViewBag.Title = "New View";

                pageSizes = new List<SelectListItem>()
                                {
                                    new SelectListItem()
                                    {
                                        Text = "25",
                                        Value = "25",
                                    },
                                    new SelectListItem()
                                    {
                                        Text = "50",
                                        Value = "50",
                                    },
                                    new SelectListItem()
                                    {
                                        Text = "100",
                                        Value = "100",
                                    }
                                };

                ViewBag.pageSizes = pageSizes;

                return PartialView("_Configuration", vm);

            }

            vm = _btsPortalRepository.LoadActivityView(id.Value);
            ViewBag.Title = vm.ViewName;
            pageSizes = new List<SelectListItem>()
                                {
                                    new SelectListItem()
                                    {
                                        Text = "25",
                                        Value = "25",
                                        Selected = vm.NoOfRowsPerPage.GetValueOrDefault(50) == 25
                                    },
                                    new SelectListItem()
                                    {
                                        Text = "50",
                                        Value = "50",
                                        Selected = vm.NoOfRowsPerPage.GetValueOrDefault(50) == 50
                                    },
                                    new SelectListItem()
                                    {
                                        Text = "100",
                                        Value = "100",
                                        Selected = vm.NoOfRowsPerPage.GetValueOrDefault(50) == 100
                                    }
                                };

            ViewBag.pageSizes = pageSizes;

            return PartialView("_Configuration", vm);


        }

        public JsonResult UpdateConfiguration(ActivityView view, List<ActivityViewFilterParameter> viewFilterParameters, bool? currentFiltersCleared)
        {
            JsonResultWrapper wrapper = new JsonResultWrapper { Success = true };
            try
            {
                if (string.IsNullOrWhiteSpace(view.ViewName))
                {
                    throw new Exception("View name is mandatory.");
                }
                if (string.IsNullOrWhiteSpace(view.SqlToExecute))
                {
                    throw new Exception("SqlToExecute is mandatory.");
                }
                if (view.SqlToExecute.ToUpper().Contains("UPDATE")
                    || view.SqlToExecute.ToUpper().Contains("DELETE")
                    || view.SqlToExecute.ToUpper().Contains("TRUNCATE"))
                {
                    throw new Exception("Suspicious sql statement. Make sure that the statement is 'select' and not any other operation.");
                }

                if (!string.IsNullOrWhiteSpace(view.SqlNoOfRows))
                {
                    if (view.SqlNoOfRows.ToUpper().Contains("UPDATE")
                    || view.SqlNoOfRows.ToUpper().Contains("DELETE")
                    || view.SqlNoOfRows.ToUpper().Contains("TRUNCATE"))
                    {
                        throw new Exception("Suspicious sql statement. Make sure that the statement is 'select' and not any other operation.");
                    }
                }

                //if (viewFilterParameters != null && viewFilterParameters.Count > 0)
                //{
                //    view.FilterParameters = string.Join(" AND ", viewFilterParameters.Select(condition => condition.ToString()).ToList());
                //}

                _btsPortalRepository.SaveActivityView(view, User.Identity.Name, viewFilterParameters, currentFiltersCleared.GetValueOrDefault(false));

                string key = string.Concat(CacheKey.IaBts_BamView.ToString(), "_", view.Id);
                _cacheProvider.Remove(key);

                int? viewId;
                if (!view.Id.HasValue || view.Id.Value == 0)
                {
                    _cacheProvider.Remove(CacheKey.IaBts_BamViews.ToString());
                    viewId =
                        _btsPortalRepository
                            .LoadActivitiesViews()
                            .FirstOrDefault(m => string.Equals(m.ViewName, view.ViewName, StringComparison.OrdinalIgnoreCase)
                            && string.Equals(m.ActivityName, view.ActivityName, StringComparison.OrdinalIgnoreCase))
                            .Id;
                }
                else
                {
                    viewId = view.Id;
                }
                wrapper.Data = viewId;
            }
            catch (Exception exception)
            {
                wrapper.Success = false;
                wrapper.Error = exception.Message;
            }
            return Json(wrapper, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ToggleConfiguration(int viewId, bool currentState)
        {
            JsonResultWrapper wrapper = new JsonResultWrapper { Success = true };
            try
            {
                _btsPortalRepository.ToggleViewConfiguration(viewId, currentState);
                _cacheProvider.Remove(CacheKey.IaBts_BamViews.ToString());
                string key = string.Concat(CacheKey.IaBts_BamView.ToString(), "_", viewId);
                _cacheProvider.Remove(key);
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