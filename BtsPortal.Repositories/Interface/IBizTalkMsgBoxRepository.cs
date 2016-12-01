using System;
using System.Collections.Generic;
using BtsPortal.Entities.Bts;

namespace BtsPortal.Repositories.Interface
{
    public interface IBizTalkMsgBoxRepository
    {
        List<Guid> GetHostInstancesStatus(List<Guid> uniqueIds);

        List<BtsInstance> GetInstances(BtsInstanceStatus status, int application, List<Guid> serviceIdsList, int page, out int totalRows, int pageSize);
        List<BtsModule> LoadApplicationModules();
        List<BtsAppArtifactSummary> LoadApplicationSummary(int appId, BtsInstanceStatus status);
        BtsSummary LoadCurrentSummary();
    }
}