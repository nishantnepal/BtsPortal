using System;
using System.Collections.Generic;
using BtsPortal.Entities.Esb;

namespace BtsPortal.Repositories.Interface
{
    public interface IEsbExceptionDbRepository
    {
        void DeleteAlert(Guid? alertId, string deletedBy);
        void DeleteAlertSubscription(Guid alertSubscriptionId);
        EsbUserAlertSubscription GetAlerts(string currentUser);
        List<EsbConfiguration> GetEsbConfiguration();
        FaultDetailAggregate GetFaultDetail(string faultId);
        FaultMessageData GetFaultMessageData(string messageId);
        MessageDetail GetFaultMessageDetail(string faultId, string messageId, int maxDataLength = 40000);
        List<FaultSearchResponse> GetFaults(FaultSearchRequest request, int page, out int totalRows, int pageSize);
        FaultSummaryVm GetFaultSummary(DateTime? startDateTime, DateTime? endDateTime);
        void SaveAlert(Guid? alertId, string alertName, string conditionString, string insertedBy, AlertType alertType, int? alertMins, List<AlertCondition> conds);
        void SaveAlertSubscription(EsbAlertSubscription alertSubscription, string currentUser);
        void ToggleAlertSubscriptionState(Guid alertSubscriptionId, bool state, string currentUser);
        void UpdateEsbConfiguration(Guid configurationId, string value, string user);
        void UpdateFaultStatus(List<string> faultIdList, FaultStatusType statusType, string currentUser, string batchId, string comment);
        AlertNotification GetAlertNotifications(int batchSize);
        void InsertAlertEmail(List<AlertEmailNotification> emailNotifications);
        void UpdateAlertLastFired(List<Guid> alertIds, DateTime lastFiredTime);
        void UpdateAlertBatchComplete(Guid batchId, string errorMsg);
        List<AlertEmailNotification> GetAlertEmails(int batchSize);
        void UpdateAlertEmails(List<AlertEmailNotification> emailNotifications);
        AlertView GetAlert(Guid? alertId);
        void UpdateAlert(Guid alertId, AlertType alertType, int? alertMins);
        AlertNotification GetAlertSummaryNotifications();
        void InsertFault(FaultDetail faultDetail, List<FaultMessage> faultMessages, List<FaultMessageContext> faultMessageContexts, List<FaultMessageData> faultMessageDatas, FaultHistory history);
    }
}