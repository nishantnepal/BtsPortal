using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BtsPortal.Entities.Bts;
using BtsPortal.Repositories.Interface;
using Dapper;

namespace BtsPortal.Repositories.Db
{
    public class BizTalkMsgBoxRepository : IBizTalkMsgBoxRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly int _commandTimeoutSeconds;
        public BizTalkMsgBoxRepository(IDbConnection dbConnection, int commandTimeoutSeconds = 600)
        {
            _dbConnection = dbConnection;
            _commandTimeoutSeconds = commandTimeoutSeconds;

        }

        public  virtual BtsSummary LoadCurrentSummary()
        {
            var vm = new BtsSummary();
            const string query = @"
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
SET DEADLOCK_PRIORITY LOW

select ApplicationId,Application,ReadyToRun,Running,Dehydrated,SuspendedResumable,SuspendedNonResumable
from
(
SELECT m.nModuleID as ApplicationId, nvcName as Application,i.[uidInstanceID],
CASE i.nState
WHEN 1 THEN 'ReadyToRun'
WHEN 2 THEN'Running'
WHEN 4 THEN 'SuspendedResumable'-- Resumable'
WHEN 8 THEN'Dehydrated'
--WHEN 16 THEN'Completed With Discarded Messages'
WHEN 32 THEN'SuspendedNonResumable'-- Non-Resumable'
ELSE 'Other'
END as State
--count(i.nState) as Count
FROM Instances i (NOLOCK)
LEFT OUTER JOIN InstancesSuspended s (NOLOCK)
on i.uidInstanceId = s.uidInstanceID
LEFT OUTER JOIN [Services] se (NOLOCK) on i.uidServiceID =  se.uidServiceID
LEFT OUTER JOIN Modules m(NOLOCK) on m.nModuleID = se.nModuleID
where ISNULL(m.nModuleID,0) <> 0
) p
PIVOT
(
COUNT (uidinstanceid)
FOR state IN
( ReadyToRun,Running,Dehydrated,SuspendedResumable,SuspendedNonResumable)
) AS pvt
order by application

SELECT m.nModuleID as ApplicationId,i.nState as InstanceStatus,
max(i.dtCreated) as MaxDate,min(i.dtCreated) as MinDate
FROM Instances i (NOLOCK)
LEFT OUTER JOIN InstancesSuspended s (NOLOCK)
on i.uidInstanceId = s.uidInstanceID
LEFT OUTER JOIN [Services] se (NOLOCK) on i.uidServiceID =  se.uidServiceID
LEFT OUTER JOIN Modules m(NOLOCK) on m.nModuleID = se.nModuleID
where ISNULL(m.nModuleID,0) <> 0
group by m.nModuleID, nvcName,i.nstate
";

            //vm.AppSummaries = _dbConnection.Query<BtsAppSummary>(query).ToList();
            using (var multi = _dbConnection.QueryMultiple(query, commandTimeout: _commandTimeoutSeconds))
            {
                vm.AppSummaries = multi.Read<BtsAppSummary>().ToList();
                vm.AppSummaryDates = multi.Read<BtsAppSummaryDate>().ToList();
            }
            return vm;
        }

        public virtual List<BtsInstance> GetInstances(BtsInstanceStatus status, int application, List<Guid> serviceIdsList, int page, out int totalRows, int pageSize)
        {
            string query;

            if (status == BtsInstanceStatus.SuspendedNonResumable || status == BtsInstanceStatus.SuspendedResumable)
            {
                query = @"
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
SET DEADLOCK_PRIORITY LOW

--declare @appId int = 5
--,@state int = 4
--,@serviceId uniqueidentifier = '87C7B1BE-F5E3-4B57-B9D7-9CDCA866F061'
--,@pageNum int = 1
--,@pageSize int = 2

select m.nModuleID as ApplicationId
,m.nvcName as Application
,nstate as InstanceStatus
,isu.uidServiceID as ServiceId
 ,uidInstanceID as InstanceId
 ,dtCreated as DateCreated
 , dtSuspendTimeStamp as DateSuspended
 --, nvcAdapter, 
 --nvcURI
 , nvcErrorDescription as ErrorDescription,
 nvcErrorProcessingServer as ProcessingServer
from InstancesSuspended isu (NOLOCK)
JOIN [Services] se (NOLOCK) on isu.uidServiceID =  se.uidServiceID
JOIN Modules m(NOLOCK) on m.nModuleID = se.nModuleID 
where m.nModuleID = @appId and nState = @state 
--and (isu.uidServiceID = ISNULL(@serviceId,isu.uidServiceID) or isu.uidServiceID in @serviceIds)
and isu.uidServiceID in @serviceIds
order by dtSuspendTimeStamp desc
OFFSET  (@pageNum -1) * @pageSize ROWS 
FETCH NEXT @pageSize ROWS ONLY ;

--total rows
select count(distinct uidInstanceID) as TotalRows
from InstancesSuspended isu (NOLOCK)
JOIN [Services] se (NOLOCK) on isu.uidServiceID =  se.uidServiceID
JOIN Modules m(NOLOCK) on m.nModuleID = se.nModuleID 
where m.nModuleID = @appId and nState = @state 
--and isu.uidServiceID = ISNULL(@serviceId,isu.uidServiceID)
and isu.uidServiceID in @serviceIds

";
            }
            else
            {
                query = @"
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
SET DEADLOCK_PRIORITY LOW

--declare @appId int = 5
--,@state int = 4
--,@serviceId uniqueidentifier = '87C7B1BE-F5E3-4B57-B9D7-9CDCA866F061'
--,@pageNum int = 1
--,@pageSize int = 2

SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
SET DEADLOCK_PRIORITY LOW

SELECT  m.nModuleID as ApplicationId
,m.nvcName as Application
,i.uidServiceID as ServiceId
,uidInstanceID as InstanceId
, dtCreated as DateCreated
, nvcProcessingServer as ProcessingServer
FROM [BizTalkMsgboxDb]..[Instances] AS i WITH (NOLOCK) 
JOIN [Services] se (NOLOCK) on i.uidServiceID =  se.uidServiceID
JOIN Modules m(NOLOCK) on m.nModuleID = se.nModuleID 
where m.nModuleID = @appId and nState = @state 
--and i.uidServiceID = ISNULL(@serviceId,i.uidServiceID)
and i.uidServiceID in @serviceIds
order by dtCreated desc
OFFSET  (@pageNum -1) * @pageSize ROWS 
FETCH NEXT @pageSize ROWS ONLY ;

--total rows
select count(distinct uidInstanceID) as TotalRows
FROM [BizTalkMsgboxDb]..[Instances] AS i WITH (NOLOCK) 
JOIN [Services] se (NOLOCK) on i.uidServiceID =  se.uidServiceID
JOIN Modules m(NOLOCK) on m.nModuleID = se.nModuleID 
where m.nModuleID = @appId and nState = @state
--and i.uidServiceID = ISNULL(@serviceId,i.uidServiceID)
and i.uidServiceID in @serviceIds

";
            }


            List<BtsInstance> responses;
            totalRows = 0;
            using (var multi = _dbConnection.QueryMultiple(query,
               new
               {
                   appId = application,
                   state = status,
                   // serviceId = request.ArtifactId,
                   serviceIds = serviceIdsList,
                   pageNum = page,
                   pageSize = pageSize

               }, commandTimeout: _commandTimeoutSeconds))
            {
                responses = multi.Read<BtsInstance>().ToList();
                totalRows = multi.Read<int>().Single();
            }

            return responses;


        }

        public virtual List<BtsAppArtifactSummary> LoadApplicationSummary(int appId, BtsInstanceStatus status)
        {

            string query;
            if (status == BtsInstanceStatus.SuspendedNonResumable || status == BtsInstanceStatus.SuspendedNonResumable)
            {
                query = @"
select m.nvcName as ApplicationName,m.nModuleID as Application,
isu.uidServiceID as ServiceId
 ,count(uidInstanceID) as InstancesCount, max(dtCreated) as DateCreatedMax,min(dtCreated) as DateCreatedMin
from InstancesSuspended isu (NOLOCK)
JOIN [Services] se (NOLOCK) on isu.uidServiceID =  se.uidServiceID
JOIN Modules m(NOLOCK) on m.nModuleID = se.nModuleID 
where m.nModuleID = @appId and nState = @state
group by isu.uidServiceID,m.nvcName,m.nModuleID
";
            }
            else
            {
                query = @"
SELECT  m.nvcName as ApplicationName,m.nModuleID as Application,
i.uidServiceID as ServiceId,
count(uidInstanceID) as InstancesCount, max(dtCreated) as DateCreatedMax,min(dtCreated) as DateCreatedMin
FROM [Instances] AS i WITH (NOLOCK) 
JOIN [Services] se (NOLOCK) on i.uidServiceID =  se.uidServiceID
JOIN Modules m(NOLOCK) on m.nModuleID = se.nModuleID 
where m.nModuleID = @appId and nState = @state
group by i.uidServiceID,m.nvcName,m.nModuleID
";
            }

            return _dbConnection.Query<BtsAppArtifactSummary>(query, new
            {
                appId = appId,
                state = status
            }).ToList();
        }

        public virtual List<BtsModule> LoadApplicationModules()
        {
            string query = @"select nModuleId as ModuleId,nvcName as ModuleName from Modules (NOLOCK)";
            return _dbConnection.Query<BtsModule>(query).ToList();
        }

        public virtual List<Guid> GetHostInstancesStatus(List<Guid> uniqueIds)
        {
            const string query = @"
SELECT uidProcessID 
FROM ProcessHeartbeats with (nolock) 
where uidProcessID in @uniqueIds

";
            return _dbConnection.Query<Guid>(query, new { uniqueIds = uniqueIds }).ToList();
        }


    }
}
