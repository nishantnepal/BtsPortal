using System;
using System.Collections.Generic;
using BtsPortal.Entities.Bts;

namespace BtsPortal.Repositories.Interface
{
    public interface IBizTalkRepository
    {
        List<BtsApplication> GetBiztalkApplications();
        BtsApplication GetBiztalkApplicationStatus(string applicationName);
        List<BtsHost> GetHosts();
        BtsInstanceDetail GetInstanceDetail(Guid instanceId, int maxBtsMessagesToShow = 50);
        BtsInstanceMessage GetInstanceMessage(Guid instanceId, Guid messageId, int maxDataLength = 40000);
        string GetInstanceMessageBody(Guid instanceId, Guid messageId);
        List<string> OrchestrationOperation(string applicationName, string orchestrationName, BtsArtifactStatus status);
        List<string> ReceiveLocationOperation(string applicationName, string portName, string locationName, bool enable);
        List<string> SendPortOperation(string applicationName, string portName, BtsArtifactStatus status);
        void StartHost(string hostName);
        void StopHost(string hostName);
        string TerminateInstance(Guid instanceId);
    }
}