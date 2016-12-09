using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BtsPortal.Entities.Bts;
using BtsPortal.Repositories.Interface;
using BtsPortal.Web.Controllers;
using BtsPortal.Web.Extensions;
using BtsPortal.Web.Infrastructure.Settings;
using BtsPortal.Web.ViewModels;

namespace BtsPortal.Web.Areas.Bts.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly IBizTalkMsgBoxRepository _bizTalkMsgBoxRepo;
        private readonly IBizTalkRepository _biztalkRepo;
        private readonly ISsoDbRepository _ssoRepo;
        private readonly IBizTalkMgmtRepository _bizTalkMgmtBoxRepo;

        public DashboardController(IBizTalkMgmtRepository bizTalkMgmtBoxRepo, IBizTalkRepository bizTalkRepository, ISsoDbRepository ssoRepo, IBizTalkMsgBoxRepository bizTalkMsgBoxRepo)
        {
            _biztalkRepo = bizTalkRepository;
            _ssoRepo = ssoRepo;
            _bizTalkMgmtBoxRepo = bizTalkMgmtBoxRepo;
            _bizTalkMsgBoxRepo = bizTalkMsgBoxRepo;
        }
        // GET: Bts/Dashboard

        public ActionResult Home()
        {
            return View();
        }

        public PartialViewResult Summary()
        {
            var vm = new BtsSummary();
            try
            {
                vm = _bizTalkMsgBoxRepo.LoadCurrentSummary();
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
            }

            return PartialView("_Summary", vm);

        }

        [HttpPost]
        public PartialViewResult AppArtifactsSummary(int appId, BtsInstanceStatus status)
        {
            var vm = new List<BtsAppArtifactSummary>();
            try
            {
                vm = _bizTalkMsgBoxRepo.LoadApplicationSummary(appId, status);
                var artifactNames = _bizTalkMgmtBoxRepo.LoadArtifactTypes();
                foreach (var instance in vm)
                {
                    var artifact = artifactNames.FirstOrDefault(m => m.ServiceId == instance.ServiceId);
                    if (artifact == null) continue;

                    instance.ArtifactName = artifact.ArtifactName;
                    instance.ArtifactType = artifact.ArtifactType;
                }

                var moduleInfo = _bizTalkMsgBoxRepo.LoadApplicationModules();
                var item = moduleInfo.FirstOrDefault(m => m.ModuleId == appId);
                ViewBag.appName = item != null ? (dynamic)item.ModuleName : appId;
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
            }

            ViewBag.status = status.ToString();
            return PartialView("_AppArtifactsSummary", vm);

        }

        public JsonResult ArtifactPerAppAndType(string appName, BtsArtifactType? artifactType)
        {
            var vm = new List<JsonKeyValue>();
            if (string.IsNullOrWhiteSpace(appName) || artifactType == null)
            {
                return Json(vm, JsonRequestBehavior.AllowGet);
            }
            var artifacts = _bizTalkMgmtBoxRepo.LoadArtifactTypes();
            vm.AddRange(from artifact in artifacts
                        where artifact.ArtifactType == artifactType && string.Equals(appName, artifact.ApplicationName, StringComparison.OrdinalIgnoreCase)
                        select new JsonKeyValue()
                        {
                            Key = artifact.ArtifactName,
                            Value = artifact.ServiceId.ToString()
                        });

            return Json(vm, JsonRequestBehavior.AllowGet);
        }


        public PartialViewResult Hosts()
        {
            var hosts = _biztalkRepo.GetHosts();
            var hostNames = hosts.Select(m => m.Name).Distinct().ToList();
            var hostInstances = _bizTalkMgmtBoxRepo.GetHostInstances(hostNames);
            var uniqueIds = hostInstances.Select(m => m.UniqueId).Distinct().ToList();
            var startedUniqueIds = _bizTalkMsgBoxRepo.GetHostInstancesStatus(uniqueIds);

            foreach (var host in hosts)
            {
                var instances = hostInstances.Where(m => m.HostName == host.Name);
                foreach (var hostInstance in instances)
                {
                    if (startedUniqueIds.Contains(hostInstance.UniqueId)
                        || host.Name.ToUpper().Contains("ISOLATED"))
                    {
                        hostInstance.HostInstanceStatus = BtsHostInstanceStatus.Started;
                    }
                    else
                    {
                        if (hostInstance.HostInstanceStatus == BtsHostInstanceStatus.Enabled)
                        {
                            hostInstance.HostInstanceStatus = BtsHostInstanceStatus.Stopped;
                        }
                    }

                    host.HostInstances.Add(hostInstance);
                }
            }

            ViewBag.Servers = hostInstances.Select(m => m.Server).Distinct().ToList();
            return PartialView("_Hosts", hosts);
        }

        public PartialViewResult Applications()
        {
            var apps = _biztalkRepo.GetBiztalkApplications();
            ViewBag.apps = apps.ToSelectListItems();
            return PartialView("_Applications");
        }

        public PartialViewResult ApplicationStatus(string applicationName)
        {
            var app = _biztalkRepo.GetBiztalkApplicationStatus(applicationName);
            return PartialView("_ApplicationStatus", app);
        }

        public PartialViewResult Sso(string selectedApp)
        {
            var apps = _ssoRepo.GetSsoApplications();
            ViewBag.apps = apps.ToSelectListItems(selected: selectedApp);
            return PartialView("_Sso");
        }

        public PartialViewResult SsoData(string appName)
        {
            var apps = _ssoRepo.GetSsoApplications();
            var contactInfo = apps.Where(m => m.Name.ToUpper() == appName.ToUpper()).Select(m => m.ContactInfo).FirstOrDefault();
            bool isBtdf = string.Equals(contactInfo, AppSettings.SsoApplicationBtdfContactEmail, StringComparison.OrdinalIgnoreCase);
            var vm = new SsoApplicationDataVm
            {
                ApplicationName = appName,
                IsBtdf = isBtdf,
                ApplicationDatas = _ssoRepo.GetSsoApplicationData(appName, isBtdf)
            };

            return PartialView("_SsoData", vm);
        }

        public PartialViewResult Edi()
        {
            var apps = _bizTalkMgmtBoxRepo.GetParties();
            ViewBag.apps = apps;
            return PartialView("_Edi");
        }

        public PartialViewResult EdiParty(string partyName)
        {
            var party = _bizTalkMgmtBoxRepo.GetParty(partyName, AppSettings.EdiHomePartyName);
            return PartialView("_EdiParty", party);
        }


    }
}