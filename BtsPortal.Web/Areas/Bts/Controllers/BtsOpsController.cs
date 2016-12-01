using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BtsPortal.Entities.Bts;
using BtsPortal.Entities.Esb;
using BtsPortal.Repositories.Interface;
using BtsPortal.Web.Models.Exceptions;
using BtsPortal.Web.ViewModels;
using BtsPortal.Web.ViewModels.Bts;

namespace BtsPortal.Web.Areas.Bts.Controllers
{
    public class BtsOpsController : Controller
    {
        private readonly IBizTalkRepository _biztalkRepo;
        private readonly ISsoDbRepository _ssoRepo;
        private readonly IBizTalkMgmtRepository _bizTalkMgmtBoxRepo;
        private readonly IEsbExceptionDbRepository _esbExceptionDbRepository;
        public BtsOpsController(IBizTalkMgmtRepository bizTalkMgmtBoxRepo, IBizTalkRepository bizTalkRepository, ISsoDbRepository ssoRepo, IEsbExceptionDbRepository esbExceptionDbRepository)
        {
            _biztalkRepo = bizTalkRepository;
            _ssoRepo = ssoRepo;
            _bizTalkMgmtBoxRepo = bizTalkMgmtBoxRepo;
            _esbExceptionDbRepository = esbExceptionDbRepository;
        }

        public JsonResult Host(BtsArtifactAction action, string hostName)
        {
            JsonResultWrapper wrapper = new JsonResultWrapper { Success = true };
            try
            {
                if (action == BtsArtifactAction.Start)
                {
                    _biztalkRepo.StartHost(hostName);
                }

                if (action == BtsArtifactAction.Stop)
                {
                    _biztalkRepo.StopHost(hostName);
                }

                if (action == BtsArtifactAction.Restart)
                {
                    _biztalkRepo.StopHost(hostName);
                    _biztalkRepo.StartHost(hostName);
                }

                wrapper.Data = "Success.";
            }
            catch (Exception exception)
            {
                wrapper.Success = false;
                wrapper.Error = exception.Message;
            }
            return Json(wrapper, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Orchestration(BtsArtifactAction action, string appName, string orchName)
        {
            JsonResultWrapper wrapper = new JsonResultWrapper { Success = true };
            try
            {
                if (action == BtsArtifactAction.Start)
                {
                    _biztalkRepo.OrchestrationOperation(appName, orchName, BtsArtifactStatus.Started);
                }

                if (action == BtsArtifactAction.Stop)
                {
                    _biztalkRepo.OrchestrationOperation(appName, orchName, BtsArtifactStatus.Enlisted);
                }

                if (action == BtsArtifactAction.Unenlist)
                {
                    _biztalkRepo.OrchestrationOperation(appName, orchName, BtsArtifactStatus.Unenlisted);
                }

                wrapper.Data = "Success.";
            }
            catch (Exception exception)
            {
                wrapper.Success = false;
                wrapper.Error = exception.Message;
            }

            return Json(wrapper, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendPort(BtsArtifactAction action, string appName, string portName)
        {
            JsonResultWrapper wrapper = new JsonResultWrapper { Success = true };
            try
            {
                if (action == BtsArtifactAction.Start)
                {
                    _biztalkRepo.SendPortOperation(appName, portName, BtsArtifactStatus.Started);
                }

                if (action == BtsArtifactAction.Stop)
                {
                    _biztalkRepo.SendPortOperation(appName, portName, BtsArtifactStatus.Stopped);
                }

                if (action == BtsArtifactAction.Unenlist)
                {
                    _biztalkRepo.SendPortOperation(appName, portName, BtsArtifactStatus.Bound);
                }

                wrapper.Data = "Success.";
            }
            catch (Exception exception)
            {
                wrapper.Success = false;
                wrapper.Error = exception.Message;
            }

            return Json(wrapper, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReceiveLocation(BtsArtifactAction action, string appName, string portName, string locationName)
        {
            JsonResultWrapper wrapper = new JsonResultWrapper { Success = true };
            var enable = action == BtsArtifactAction.Start;
            try
            {
                _biztalkRepo.ReceiveLocationOperation(appName, portName, locationName, enable);
                wrapper.Data = "Success.";
            }
            catch (Exception exception)
            {
                wrapper.Success = false;
                wrapper.Error = exception.Message;
            }

            return Json(wrapper, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveSsoSetting(string appName, bool isBtdf, string key, string value, string orgValue)
        {
            JsonResultWrapper wrapper = new JsonResultWrapper { Success = true };
            try
            {
                _ssoRepo.SaveSsoSetting(appName, isBtdf, key, value);
                wrapper.Data = "Success.";
            }
            catch (Exception exception)
            {
                wrapper.Success = false;
                wrapper.Error = exception.Message;
            }

            return Json(wrapper, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult CreateNewSsoApp(string appName)
        //{
        //    JsonResultWrapper wrapper = new JsonResultWrapper { Success = true };
        //    try
        //    {
        //        _ssoRepo.CreateNewApp(appName);
        //        wrapper.Data = "Success.";
        //    }
        //    catch (Exception exception)
        //    {
        //        wrapper.Success = false;
        //        wrapper.Error = exception.Message;
        //    }

        //    return Json(wrapper, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult UpdateEdiBatchStatus(BtsEdiBatchOrchestrationAction action, int batchId, string batchName, string agreementName, string sendingPartner, string receivingPartner, int oneWayAgreementId)
        {
            JsonResultWrapper wrapper = new JsonResultWrapper { Success = true };
            try
            {
                if (action != BtsEdiBatchOrchestrationAction.Refresh)
                {
                    if (_bizTalkMgmtBoxRepo.IsEdiControlMessageProcessing(batchId))
                    {
                        throw new Exception("There is already a control message pending processing.Please refresh.");
                    }
                    _bizTalkMgmtBoxRepo.UpdateEdiBatchStatus(action, batchId, batchName, agreementName, sendingPartner, receivingPartner);
                }

                var dbBatch = _bizTalkMgmtBoxRepo.GetEdiBatchingOrchestrationInstanceIds(oneWayAgreementId);
                var btsBatch = new BtsPartyBusinessProfileOneWayAgreementBatch()
                {
                    Name = batchName,
                    Id = batchId,
                    OrchestrationId = dbBatch.FirstOrDefault(m => m.BatchName == batchName)?.BatchOrchestrationId,
                    PendingStart = dbBatch.Any(m => m.BatchName == batchName) && dbBatch.FirstOrDefault(m => m.BatchName == batchName)?.BatchOrchestrationId == null,
                    PendingPamControlOperation = dbBatch.FirstOrDefault(m => m.BatchName == batchName)?.ControlBatchId > 0
                };
                wrapper.Data = btsBatch;
            }
            catch (Exception exception)
            {
                wrapper.Success = false;
                wrapper.Error = exception.Message;
            }

            return Json(wrapper, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MigrateInstancesToEsb(List<string> instanceIds)
        {
            const string defaultText = "Terminated using BtsPortal";
            JsonResultWrapper wrapper = new JsonResultWrapper { Success = true };
            List<string> errorList = new List<string>();
            int succeeded = 0;
            int failed = 0;
            int pending = 0;
            try
            {
                //1. get biztalk data
                foreach (string instId in instanceIds)
                {
                    List<FaultMessage> faultMessages = new List<FaultMessage>();
                    List<FaultMessageContext> faultMessageContexts = new List<FaultMessageContext>();
                    List<FaultMessageData> faultMessageDatas = new List<FaultMessageData>();
                    Guid instanceId;
                    if (Guid.TryParse(instId, out instanceId))
                    {
                        try
                        {
                            Guid faultId = Guid.NewGuid();
                            BtsInstanceDetail btsInstanceDetail = _biztalkRepo.GetInstanceDetail(instanceId, 5000);
                            TerminateAndMigrateToEsbException exception = new TerminateAndMigrateToEsbException(btsInstanceDetail.ErrorDescription);
                            var faultDetail = new FaultDetail()
                            {
                                Application = btsInstanceDetail.Application,
                                DateTime = btsInstanceDetail.CreateDateTime,
                                FailureCategory = btsInstanceDetail.ServiceClass,
                                FaultCode = btsInstanceDetail.ErrorCode,
                                FaultGenerator = btsInstanceDetail.ServiceClass,
                                Description = btsInstanceDetail.ErrorDescription,
                                ExceptionSource = btsInstanceDetail.ServiceName,
                                FaultId = faultId,
                                ExceptionMessage = btsInstanceDetail.ErrorDescription,
                                ExceptionStackTrace = defaultText,
                                InsertMessagesFlag = btsInstanceDetail.Messages.Count > 0,
                                Scope = defaultText,
                                FaultSeverity = 3,
                                ServiceInstanceID = instanceId.ToString(),
                                ServiceName = btsInstanceDetail.ServiceName,
                                MachineName = btsInstanceDetail.Server,
                                ExceptionType = exception.GetType().ToString(),
                                ExceptionTargetSite = "",
                                InnerExceptionMessage = "",
                                InsertedDate = DateTime.UtcNow,
                                ErrorType = exception.GetType().Name,
                                FaultDescription = btsInstanceDetail.ErrorDescription,
                                ActivityID = string.Empty,
                                NativeMessageID = Guid.NewGuid().ToString()

                            };
                            FaultHistory history = new FaultHistory()
                            {
                                FaultId = faultId,
                                FaultStatusType = FaultStatusType.UnResolved,
                                Comment = "Terminated in BTS and migrated to ESB.",
                                UpdatedBy = User.Identity.Name,
                                UpdatedTimeUtc = DateTime.UtcNow
                            };

                            foreach (var btsInstanceMessage in btsInstanceDetail.Messages)
                            {
                                var messageId = Guid.NewGuid();
                                var detail = _biztalkRepo.GetInstanceMessage(instanceId,
                                    new Guid(btsInstanceMessage.MessageId), int.MaxValue);

                                faultMessages.Add(new FaultMessage()
                                {
                                    FaultId = faultId,
                                    ContentType = "text/plain",
                                    InterchangeID = "",
                                    MessageID = messageId,
                                    MessageName = !string.IsNullOrWhiteSpace(detail.Name) ? detail.Name : btsInstanceMessage.Name,
                                    NativeMessageId = detail.MessageId,

                                });

                                faultMessageDatas.Add(new FaultMessageData()
                                {
                                    MessageID = messageId,
                                    MessageData = detail.MessageBody
                                });

                                faultMessageContexts.AddRange(detail.Contexts.Select(context => new FaultMessageContext()
                                {
                                    Name = context.Name,
                                    MessageID = messageId,
                                    ContextPropertyId = Guid.NewGuid(),
                                    Type = context.Namespace,
                                    Value = context.Value
                                }));
                            }

                            //2. insert into esb
                            _esbExceptionDbRepository.InsertFault(faultDetail, faultMessages, faultMessageContexts, faultMessageDatas, history);
                            //3. terminate instance
                            var status = _biztalkRepo.TerminateInstance(instanceId);
                            switch (status)
                            {
                                case "Succeeded":
                                    succeeded++;
                                    break;
                                case "Failed":
                                    failed++;
                                    break;
                                case "Pending":
                                    pending++;
                                    break;
                            }

                        }
                        catch (Exception exception)
                        {
                            errorList.Add($"'{instId}' failed : {exception.Message}");
                            failed++;
                        }
                    }
                }

                wrapper.Data = errorList.Count > 0 
                    ? $"Total : {instanceIds.Count}. Succeeded : {succeeded}. Failed : {failed}. Pending: {pending}. <br/> Errors: {string.Join("<br/>", errorList)}" 
                    : $"Total : {instanceIds.Count}. Succeeded : {succeeded}. Failed : {failed}. Pending: {pending}.";
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