using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using BtsPortal.Entities.Bts;
using BtsPortal.Repositories.Interface;
using BtsPortal.Web.Controllers;
using BtsPortal.Web.Extensions;
using BtsPortal.Web.Infrastructure.Settings;
using BtsPortal.Web.ViewModels.Bts;

namespace BtsPortal.Web.Areas.Bts.Controllers
{
    public class DetailController : BaseController
    {
        private readonly IBizTalkMsgBoxRepository _bizTalkMsgBoxRepo;
        private readonly IBizTalkMgmtRepository _bizTalkMgmtBoxRepo;
        private readonly IBizTalkRepository _biztalkRepo;

        public DetailController(IBizTalkMgmtRepository bizTalkMgmtBoxRepo, IBizTalkRepository bizTalkRepository, IBizTalkMsgBoxRepository bizTalkMsgBoxRepo)
        {
            _bizTalkMsgBoxRepo = bizTalkMsgBoxRepo;
            _bizTalkMgmtBoxRepo = bizTalkMgmtBoxRepo;
            _biztalkRepo = bizTalkRepository;
        }

        public ActionResult Index(BtsSearchRequest request, int? page)
        {
            var vm = new BtsSearchRequestResponse();
            List<BtsArtifact> artifacts = new List<BtsArtifact>();
            try
            {
                if (request == null || request.Init == false)
                {
                    vm.Request = new BtsSearchRequest()
                    {
                        Init = true,
                        Applications = _bizTalkMsgBoxRepo.LoadApplicationModules().ToSelectListItems(null)
                    };

                    return View(vm);
                }

                artifacts = _bizTalkMgmtBoxRepo.LoadArtifactTypes();
                var appModules = _bizTalkMsgBoxRepo.LoadApplicationModules();
                string appName = appModules
                        .Where(m => m.ModuleId == request.Application)
                        .FirstOrDefault()
                        .ModuleName;

                vm.Request = request;
                vm.Request.Applications = appModules.ToSelectListItems(request.Application);
                vm.Request.ArtifactIds.Add(new SelectListItem()
                {
                    Text = "--All--",
                    Value = string.Empty
                });

                vm.Request.ArtifactIds.AddRange(from artifact in artifacts
                                                where artifact.ArtifactType == request.ArtifactType && string.Equals(appName, artifact.ApplicationName, StringComparison.OrdinalIgnoreCase)
                                                select new SelectListItem()
                                                {
                                                    Text = artifact.ArtifactName,
                                                    Value = artifact.ServiceId.ToString(),
                                                    Selected = artifact.ServiceId == request.ArtifactId
                                                });

                if (request.Status == 0)
                {
                    throw new Exception("Status is required");
                }

                List<Guid> serviceidsList = null;



                if (request.ArtifactType != null && request.ArtifactId != null)
                {
                    serviceidsList = artifacts.Where(
                        m =>
                            m.ArtifactType == request.ArtifactType &&
                            string.Equals(m.ApplicationName, appName, StringComparison.OrdinalIgnoreCase)
                            && request.ArtifactId == m.ServiceId)
                        .Select(m => m.ServiceId)
                        .ToList();
                }

                if (request.ArtifactType == null)
                {
                    serviceidsList = artifacts.Where(
                        m =>
                            string.Equals(m.ApplicationName, appName, StringComparison.OrdinalIgnoreCase))
                        .Select(m => m.ServiceId)
                        .ToList();
                }

                if (request.ArtifactType != null && request.ArtifactId == null)
                {
                    serviceidsList = artifacts.Where(
                        m =>
                            m.ArtifactType == request.ArtifactType &&
                            string.Equals(m.ApplicationName, appName, StringComparison.OrdinalIgnoreCase))
                        .Select(m => m.ServiceId)
                        .ToList();
                }

                int totalRows;
                var responses = _bizTalkMsgBoxRepo.GetInstances(request.Status, request.Application, serviceidsList, page.GetValueOrDefault(1), out totalRows, AppSettings.PageSize);
                foreach (var instance in responses)
                {
                    var artifact = artifacts.FirstOrDefault(m => m.ServiceId == instance.ServiceId);
                    if (artifact == null) continue;

                    instance.ArtifactName = artifact.ArtifactName;
                    instance.ArtifactType = artifact.ArtifactType;
                }

                vm = new BtsSearchRequestResponse()
                {
                    Request = request,
                    Responses = responses,
                    PageSize = AppSettings.PageSize,
                    Page = page,
                    TotalRows = totalRows

                };

            }
            catch (Exception exception)
            {
                vm.Request = request;
                vm.Error = exception.Message;
            }

            return View(vm);
        }

        [HttpPost]
        public PartialViewResult InstanceDetail(Guid? instanceId)
        {
            var vm = new BtsInstanceDetail()
            {
                InstanceId = instanceId.GetValueOrDefault().ToString()
            };
            try
            {
                if (!instanceId.HasValue)
                {
                    throw new Exception("Invalid instance id.");
                }

                vm = _biztalkRepo.GetInstanceDetail(instanceId.Value);
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
            }

            return PartialView("_InstanceDetail", vm);
        }

        public PartialViewResult MessageDetail(Guid instanceId, Guid messageId)
        {
            BtsInstanceMessage vm = new BtsInstanceMessage();
            try
            {
                vm = _biztalkRepo.GetInstanceMessage(instanceId, messageId);
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
            }

            return PartialView("_MessageDetail", vm);
        }

        public ActionResult DownloadMessage(Guid instanceId, Guid messageId)
        {
            string data;
            try
            {
                data = _biztalkRepo.GetInstanceMessageBody(instanceId, messageId);

            }
            catch (Exception exception)
            {
                data = "Error - " + exception.Message;
            }
            string fileName = string.Format(data.StartsWith("<") ? "{0}.xml" : "{0}.dat", Guid.NewGuid().ToString());
            string contentType = data.StartsWith("<") ? "text/xml" : "text/plain";
            return File(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data)), contentType, fileName);
        }
    }
}