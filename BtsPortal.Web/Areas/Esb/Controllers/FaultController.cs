using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using BtsPortal.Entities.Esb;
using BtsPortal.Repositories.Interface;
using BtsPortal.Web.Controllers;
using BtsPortal.Web.Extensions;
using BtsPortal.Web.Infrastructure.Settings;
using BtsPortal.Web.ViewModels;
using BtsPortal.Web.ViewModels.Esb;

namespace BtsPortal.Web.Areas.Esb.Controllers
{
    public class FaultController : BaseController
    {
        private readonly IEsbExceptionDbRepository _esbExcRepository;
        private readonly IBizTalkRepository _btsRepository;
        public FaultController(IEsbExceptionDbRepository esbExceptionDbRepository, IBizTalkRepository bizTalkRepository)
        {
            _esbExcRepository = esbExceptionDbRepository;
            _btsRepository = bizTalkRepository;
        }

        // GET: Esb/Fault
        public ActionResult Index(FaultSearchRequestVm request, int? page)
        {
            var vm = new FaultSearchRequestResponse();

            if (request == null || request.Init == false)
            {
                vm.Request = new FaultSearchRequestVm()
                {
                    Init = true,
                    FromDateTime = DateTime.Today,
                    ToDateTime = DateTime.Now,
                    Applications = _btsRepository.GetBiztalkApplications().ToSelectListItems()
                };

                return View(vm);
            }
            //FaultSearchRequest request, int page, out int totalRows, int pageSize
            int totalRows;
            var responses = _esbExcRepository.GetFaults(request, page.GetValueOrDefault(1), out totalRows, AppSettings.PageSize);
            vm = new FaultSearchRequestResponse
            {
                Request = request,
                Page = page.GetValueOrDefault(1),
                PageSize = AppSettings.PageSize,
                Responses = responses,
                TotalRows = totalRows
            };
            vm.Request.Applications = _btsRepository.GetBiztalkApplications().ToSelectListItems(selected: request.Application);

            return View(vm);
        }

        public ActionResult Summary(DateTime? startDate, DateTime? endDate)
        {
            var vm = new FaultSummaryVm();
            if (!startDate.HasValue || !endDate.HasValue)
            {
                vm.StartDate = DateTime.Today;
                vm.EndDate = DateTime.Now;
                return View(vm);
            }
            vm = _esbExcRepository.GetFaultSummary(startDate.ToUtcDateTime(), endDate.ToUtcDateTime());
            return View(vm);
        }

        public PartialViewResult Detail(string faultId)
        {
            var vm = _esbExcRepository.GetFaultDetail(faultId);
            return PartialView("_Detail", vm);
        }

        public PartialViewResult MessageDetail(string faultId, string messageId)
        {
            var vm = _esbExcRepository.GetFaultMessageDetail(faultId, messageId);
            return PartialView("_MessageDetail", vm);
        }

        [HttpPost]
        public JsonResult UpdateFaultStatus(FaultStatusType statusType, List<string> faultIds, string comment)
        {
            JsonResultWrapper wrapper = new JsonResultWrapper { Success = true };
            var batchId = Guid.NewGuid().ToString();
            try
            {
                _esbExcRepository.UpdateFaultStatus(faultIds, statusType, User.Identity.Name, batchId, comment);
                if (statusType == FaultStatusType.Resolved)
                {
                    wrapper.Data = "Successfully resolved the seleted fault(s).Please reload results for the most current update.";
                }
                else
                {
                    wrapper.Data = "Successfully flagged the seleted fault(s).Please reload results for the most current update.";
                }
            }
            catch (Exception exception)
            {
                wrapper.Success = false;
                wrapper.Error = exception.Message;
            }

            return Json(wrapper, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadFile(string messageId)
        {
            var vm = _esbExcRepository.GetFaultMessageData(messageId);
            string fileName = string.Format(vm.MessageData.StartsWith("<") ? "{0}.xml" : "{0}.dat", Guid.NewGuid().ToString());
            string contentType = vm.MessageData.StartsWith("<") ? "text/xml" : "text/plain";
            return File(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(vm.MessageData)), contentType, fileName);
        }
    }
}