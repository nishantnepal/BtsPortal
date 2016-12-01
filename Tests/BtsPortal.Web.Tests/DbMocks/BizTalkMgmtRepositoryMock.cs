using System;
using System.Collections.Generic;
using BtsPortal.Entities.Bts;
using BtsPortal.Repositories.Interface;

namespace BtsPortal.Web.Tests.DbMocks
{
    public class BizTalkMgmtRepositoryMock : IBizTalkMgmtRepository
    {
        public BizTalkMgmtRepositoryMock()
        {

        }

        public bool IsEdiControlMessageProcessing(int batchId)
        {
            throw new NotImplementedException();
        }

        public List<DbOneWayAgreementBatchingOrch> GetEdiBatchingOrchestrationInstanceIds(int oneWayAgreementId)
        {
            throw new NotImplementedException();
        }

        public List<BtsHostInstance> GetHostInstances(List<string> hostNames)
        {
            throw new NotImplementedException();
        }

        public List<BtsParty> GetParties()
        {
            throw new NotImplementedException();
        }

        public List<BtsArtifact> LoadArtifactTypes()
        {
            throw new NotImplementedException();
        }

        public BtsParty GetParty(string partyName, string homeParty)
        {
            throw new NotImplementedException();
        }

        public void UpdateEdiBatchStatus(BtsEdiBatchOrchestrationAction action, int batchId, string batchName, string agreementName,
            string sendingPartner, string receivingPartner)
        {
            throw new NotImplementedException();
        }
    }
}
