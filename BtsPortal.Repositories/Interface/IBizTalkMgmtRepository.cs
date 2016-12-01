using System.Collections.Generic;
using BtsPortal.Entities.Bts;

namespace BtsPortal.Repositories.Interface
{
    public interface IBizTalkMgmtRepository
    {
        bool IsEdiControlMessageProcessing(int batchId);
        List<DbOneWayAgreementBatchingOrch> GetEdiBatchingOrchestrationInstanceIds(int oneWayAgreementId);
        List<BtsHostInstance> GetHostInstances(List<string> hostNames);
        List<BtsParty> GetParties();
        List<BtsArtifact> LoadArtifactTypes();
        BtsParty GetParty(string partyName, string homeParty);
        void UpdateEdiBatchStatus(BtsEdiBatchOrchestrationAction action, int batchId, string batchName, string agreementName, string sendingPartner, string receivingPartner);
    }
}