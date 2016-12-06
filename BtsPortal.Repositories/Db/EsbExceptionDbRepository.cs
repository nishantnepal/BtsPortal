using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using BtsPortal.Entities.Esb;
using BtsPortal.Repositories.Interface;
using BtsPortal.Repositories.Utilities;
using Dapper;

namespace BtsPortal.Repositories.Db
{
    public class EsbExceptionDbRepository : IEsbExceptionDbRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly int _commandTimeoutSeconds;
        public EsbExceptionDbRepository(IDbConnection dbConnection, int commandTimeoutSeconds = 600)
        {
            _dbConnection = dbConnection;
            _commandTimeoutSeconds = commandTimeoutSeconds;

        }

        public FaultSummaryVm GetFaultSummary(DateTime? startDateTime, DateTime? endDateTime)
        {
            var vm = new FaultSummaryVm()
            {
                StartDate = startDateTime,
                EndDate = endDateTime
            };
            const string query = @"
--Declare @startDateTime datetime, @endDateTime datetime

select application,UnResolved,Resolved,Resubmitted,Flagged,LastFaultOccurredDateUtc
from
(
select f.application,
Isnull(fst.[Name],'UnResolved') as FaultStatus
,f.FaultID
,md.LastFaultOccurredDateUtc
from Fault f (NOLOCK)
left outer join [dbo].[Portal_FaultStatus] fs (NOLOCK) on f.FaultID = fs.FaultId
left outer join [dbo].[Portal_FaultStatusType] fst (NOLOCK) on fs.FaultStatusTypeID = fst.Id
left outer join
(
	select f.application
	, max([DateTime]) as LastFaultOccurredDateUtc
	from Fault f (NOLOCK)
	where DateTime between ISNULL(@startDateTime,'2000-01-01') and ISNULL(@endDateTime,'2099-01-01')
	group by f.application
) as md on f.Application = md.Application
where DateTime between ISNULL(@startDateTime,'2000-01-01') and ISNULL(@endDateTime,'2099-01-01')
) p
PIVOT
(
COUNT (faultid)
FOR FaultStatus IN
( UnResolved,Resolved,Resubmitted,Flagged )
) AS pvt
ORDER BY application

--select ErrorType,UnResolved,Resolved,Resubmitted,Flagged,LastFaultOccurredDateUtc
--from
--(
--select f.ErrorType,
--Isnull(fst.[Name],'UnResolved') as FaultStatus
--,f.FaultID
--,md.LastFaultOccurredDateUtc
--from Fault f (NOLOCK)
--left outer join [dbo].[Portal_FaultStatus] fs (NOLOCK) on f.FaultID = fs.FaultId
--left outer join [dbo].[Portal_FaultStatusType] fst (NOLOCK) on fs.FaultStatusTypeID = fst.Id
--left outer join
--(
--	select f.ErrorType
--	, max([DateTime]) as LastFaultOccurredDateUtc
--	from Fault f (NOLOCK)
--	where DateTime between ISNULL(@startDateTime,'2000-01-01') and ISNULL(@endDateTime,'2099-01-01')
--	group by f.ErrorType
--) as md on f.ErrorType = md.ErrorType
--where DateTime between ISNULL(@startDateTime,'2000-01-01') and ISNULL(@endDateTime,'2099-01-01')
--) p
--PIVOT
--(
--COUNT (faultid)
--FOR FaultStatus IN
--(UnResolved,Resolved,Resubmitted,Flagged )
--) AS pvt
--ORDER BY ErrorType
";

            var data = _dbConnection.QueryMultiple(query, new { startDateTime, endDateTime });
            vm.FaultSummariesByApplication = data.Read<FaultSummary>() as List<FaultSummary>;
            //vm.FaultSummariesByErrorType = data.Read<FaultSummary>() as List<FaultSummary>;

            return vm;
        }

        public List<FaultSearchResponse> GetFaults(FaultSearchRequest request, int page, out int totalRows, int pageSize)
        {
            const string query = @"
--Declare @fromDateTime datetime,
--	@toDateTime datetime,
--	@faultStatusTypeId int,
--	@errorType varchar(100),
--  @errorTypeExact varchar(100),
--	@faultCode varchar(100),
--	@failureCategory varchar(100),
--	@failureScope varchar(100),
--	@faultId varchar(50),
--	@message varchar(100),
--	@application varchar(100),
--	@pageNum int = 1,
--	@pageSize int = 100

--set @fromDateTime = '08/01/2015'
--set @toDateTime = GETDATE()
--set @application = 'Match_Claims'
----set @faultId = '0de3b300-d856-4158-9992-3ec0eace8729'
--set @message = '000000382'

select distinct
Application, 
FaultSeverity, 
DateTime as TransactionDateTimeUtc,
f.FaultID, 
ErrorType, 
FailureCategory, 
FaultCode, 
Scope, 
ServiceName, 
FaultGenerator, 
MachineName,
ISNULL(fs.FaultStatusTypeId,1) as Status,
f.InsertMessagesFlag
from Fault f (NOLOCK)
left outer join Portal_FaultStatus fs (NOLOCK) on f.FaultID = fs.FaultID
left outer join Message m (NOLOCK) on f.FaultID = m.FaultID
left outer join MessageData md (NOLOCK) on m.MessageID = md.MessageID
where isnull(f.Application,'') = ISNULL(@application,f.Application)
and f.DateTime between ISNULL(@fromDateTime,'2000-01-01') and ISNULL(@toDateTime,'2099-01-01')
and ISNULL(fs.FaultStatusTypeId,1) = ISNULL(@faultStatusTypeId,ISNULL(fs.FaultStatusTypeId,1))
and cast(f.FaultID as varchar(50)) like '%' + ISNULL(@faultId,cast(f.FaultID as varchar(50))) + '%'
and isnull(f.ErrorType,'')  like '%' + ISNULL(@errorType,f.ErrorType) + '%'
and isnull(f.ErrorType,'') = ISNULL(@errorTypeExact,f.ErrorType)
and isnull(f.FaultCode,'')  like '%' + ISNULL(@faultCode,f.FaultCode) + '%'
and isnull(md.MessageData,'')  like '%' + ISNULL(@message,md.MessageData) + '%'
and isnull(f.FailureCategory,'')  like '%' + ISNULL(@failureCategory,f.FailureCategory) + '%'
and isnull(f.Scope,'')  like '%' + ISNULL(@failureScope,f.Scope) + '%'
order by f.DateTime desc
OFFSET  (@pageNum -1) * @pageSize ROWS 
FETCH NEXT @pageSize ROWS ONLY ;

--total rows
select count(distinct f.FaultID) as TotalRows
from Fault f (NOLOCK) 
left outer join Portal_FaultStatus fs (NOLOCK) on f.FaultID = fs.FaultID
left outer join [dbo].[Portal_FaultStatusType] fst (NOLOCK) on fs.FaultStatusTypeId=fst.Id
left outer join Message m (NOLOCK) on f.FaultID = m.FaultID
left outer join MessageData md (NOLOCK) on m.MessageID = md.MessageID
where isnull(f.Application,'') = ISNULL(@application,f.Application)
and f.DateTime between ISNULL(@fromDateTime,'2000-01-01') and ISNULL(@toDateTime,'2099-01-01')
and ISNULL(fs.FaultStatusTypeId,1) = ISNULL(@faultStatusTypeId,ISNULL(fs.FaultStatusTypeId,1))
and cast(f.FaultID as varchar(50)) like '%' + ISNULL(@faultId,cast(f.FaultID as varchar(50))) + '%'
and isnull(f.ErrorType,'')  like '%' + ISNULL(@errorType,f.ErrorType) + '%'
and isnull(f.FaultCode,'')  like '%' + ISNULL(@faultCode,f.FaultCode) + '%'
and isnull(md.MessageData,'')  like '%' + ISNULL(@message,md.MessageData) + '%'
and isnull(f.FailureCategory,'')  like '%' + ISNULL(@failureCategory,f.FailureCategory) + '%'
and isnull(f.Scope,'')  like '%' + ISNULL(@failureScope,f.Scope) + '%'
";

            List<FaultSearchResponse> responses;
            totalRows = 0;

            using (var multi = _dbConnection.QueryMultiple(query,
                new
                {
                    application = request.Application.ToDbStringAnsi(),
                    fromDateTime = request.FromDateTime,
                    toDateTime = request.ToDateTime,
                    faultStatusTypeId = request.Status,
                    errorType = request.ErrorType.ToDbStringAnsi(),
                    faultCode = request.FaultCode.ToDbStringAnsi(),
                    failureCategory = request.FailureCategory.ToDbStringAnsi(),
                    failureScope = request.FailureScope.ToDbStringAnsi(),
                    faultId = request.FaultId.ToDbStringAnsi(),
                    message = request.Message.ToDbStringAnsi(),
                    pageSize = pageSize,
                    errorTypeExact = request.ErrorTypeExact,
                    pageNum = page

                }, commandTimeout: _commandTimeoutSeconds))
            {
                responses = multi.Read<FaultSearchResponse>().ToList();
                totalRows = multi.Read<int>().Single();
            }

            return responses;

        }

        public FaultDetailAggregate GetFaultDetail(string faultId)
        {
            const string query = @"
--declare @faultId varchar(50)
--set @faultId = '13471b20-cb1a-412d-996a-4b58126284dd'

select
FaultID, 
NativeMessageID, 
ActivityID, 
Application, 
Description, 
ErrorType, 
FailureCategory, 
FaultCode, 
FaultDescription, 
FaultSeverity, 
Scope, 
ServiceInstanceID, 
ServiceName, 
FaultGenerator, 
MachineName, 
DateTime, 
ExceptionMessage, 
ExceptionType, 
ExceptionSource, 
ExceptionTargetSite, 
ExceptionStackTrace, 
InnerExceptionMessage, 
InsertMessagesFlag, 
InsertedDate
from Fault f (NOLOCK)
where f.FaultID = @faultId

select FaultID,MessageID,InterchangeID,MessageName,ContentType
from Message (NOLOCK)
where FaultID = @faultId

select distinct FaultID, FaultStatusTypeId as FaultStatusType, UpdatedTime as UpdatedTimeUtc, UpdatedBy,Comment
from Portal_FaultHistory (NOLOCK)
where FaultID = @faultId

";

            var vm = new FaultDetailAggregate();
            using (var multi = _dbConnection.QueryMultiple(query,
                new
                {
                    faultId = faultId.ToDbStringAnsi()
                }))
            {
                vm.FaultDetail = multi.Read<FaultDetail>().SingleOrDefault();
                vm.FaultMessages = multi.Read<FaultMessage>().ToList();
                vm.FaultHistories = multi.Read<FaultHistory>().ToList();
            }

            return vm;
        }

        public void UpdateFaultStatus(List<string> faultIdList, FaultStatusType statusType, string currentUser,
            string batchId, string comment)
        {
            const string query = @"
--declare @faultId varchar(50),@faultStatusTypeId smallInt,@currentUser varchar(50),@batchId uniqueidentifier,@comment varchar(50)

MERGE Portal_FaultStatus AS target  
    USING (SELECT '{0}', '{1}') AS source (FaultId, FaultStatusTypeId)  
    ON (target.FaultId = source.FaultId)  
    WHEN MATCHED THEN   
        UPDATE SET FaultStatusTypeId = source.FaultStatusTypeId 
WHEN NOT MATCHED THEN  
    INSERT (FaultID, FaultStatusTypeId)  
    VALUES (source.FaultID, source.FaultStatusTypeId);

insert into [dbo].[Portal_FaultHistory](FaultID, FaultStatusTypeId, UpdatedTime, UpdatedBy, UpdatedBatchId,Comment)
values(
'{0}',
'{1}',
GETUTCDATE(),
'{2}',
'{3}',
'{4}'
);

";

            StringBuilder sqlStatements = new StringBuilder();

            foreach (string faultId in faultIdList)
            {
                sqlStatements.AppendLine(string.Format(query, faultId, (int)statusType, currentUser, batchId, comment));
            }

            int result = _dbConnection.Execute(sqlStatements.ToString(), null, null, _commandTimeoutSeconds);

        }

        public MessageDetail GetFaultMessageDetail(string faultId, string messageId, int maxDataLength = 40000)
        {
            const string query = @"
--declare @faultid uniqueidentifier = 'b429c1a2-3e57-4734-8567-0c6d957fac0a'
--	   ,@messageId uniqueidentifier = '6b81fb78-d8be-48fb-83fc-9bfb0e05c721'
--	   ,@maxDataLength int = 40000

select FaultId,MessageID,ContentType,MessageName,InterchangeID
from Message (NOLOCK)
where CONVERT(varchar(36), FaultID) = @faultid
and CONVERT(varchar(36), MessageID) = @messageId

select MessageID,SUBSTRING ( MessageData ,0 , @maxDataLength ) as MessageData,DATALENGTH(MessageData) as MessageDataLength,@maxDataLength as MaxDataLength
from MessageData (NOLOCK)
where CONVERT(varchar(36), MessageID) = @messageId

select name,Value,Type
 from ContextProperty (NOLOCK)
where CONVERT(varchar(36), MessageID) = @messageId

";

            var vm = new MessageDetail();
            using (var multi = _dbConnection.QueryMultiple(query,
                new
                {
                    faultId = faultId.ToDbStringAnsi(),
                    messageId = messageId.ToDbStringAnsi(),
                    maxDataLength = maxDataLength
                }))
            {
                vm.Message = multi.Read<FaultMessage>().SingleOrDefault();
                vm.Data = multi.Read<FaultMessageData>().SingleOrDefault();
                vm.FaultMessageContexts = multi.Read<FaultMessageContext>().ToList();
            }

            return vm;
        }

        public FaultMessageData GetFaultMessageData(string messageId)
        {
            const string query = @"
--declare @messageId uniqueidentifier = '6b81fb78-d8be-48fb-83fc-9bfb0e05c721'
select MessageData
from MessageData (NOLOCK)
where CONVERT(varchar(36), MessageID) = @messageId
";

            return _dbConnection.Query<FaultMessageData>(query, new
            {
                messageId = messageId.ToDbStringAnsi()
            }).SingleOrDefault();
        }

        public EsbUserAlertSubscription GetAlerts(string currentUser)
        {
            const string query = @"
--declare @subscriber varchar(50)='AzureAD\NishantNepal'

SELECT     
	a.[AlertID], 
	[Name], 
	[ConditionsString], 
	[InsertedBy],
	[InsertedDate],
	(SELECT COUNT(AlertID) FROM dbo.AlertSubscription s WHERE AlertID = a.AlertID and active=1) AS TotalSubscriberCount,
	ISNULL(ia.[IsSummaryAlert],0) as IsSummaryAlert
	,ia.[LastFired]
	,ia.[IntervalMins]
FROM dbo.Alert a (NOLOCK)
left outer join [dbo].[Portal_Alert] ia (NOLOCK) on a.AlertID = ia.alertid

select asu.alertid,asu.AlertSubscriptionID,a.Name,a.ConditionsString,asu.Active,asu.CustomEmail,asu.Subscriber,asu.[InsertedDate],asu.[InsertedBy]
from AlertSubscription asu(NOLOCK)
inner join Alert a (NOLOCK)on a.AlertID = asu.AlertID
where Subscriber = @subscriber
order by Active,name,asu.Subscriber,CustomEmail


select distinct asu.alertid,asu.AlertSubscriptionID,asu.CustomEmail,asu.Subscriber
from AlertSubscription asu(NOLOCK)
inner join Alert a (NOLOCK)on a.AlertID = asu.AlertID
where Active = 1
order by asu.Subscriber,CustomEmail
";

            var vm = new EsbUserAlertSubscription()
            {
                CurrentUser = currentUser
            };
            List<EsbAlertSubscription> alertSubs = new List<EsbAlertSubscription>();
            using (var multi = _dbConnection.QueryMultiple(query,
                new
                {
                    subscriber = currentUser.ToDbStringAnsi(),
                }))
            {
                vm.Alerts = multi.Read<EsbAlert>().ToList();
                vm.MyAlertSubscriptions = multi.Read<EsbAlertSubscription>().ToList();
                alertSubs = multi.Read<EsbAlertSubscription>().ToList();
            }

            foreach (var alertSub in alertSubs)
            {
                var alert = vm.Alerts.FirstOrDefault(m => m.AlertId == alertSub.AlertId);
                alert?.AlertSubscriptions.Add(new EsbAlertSubscription()
                {
                    Active = true,
                    AlertId = alert.AlertId,
                    AlertSubscriptionId = alertSub.AlertSubscriptionId,
                    CustomEmail = alertSub.CustomEmail,
                    Subscriber = alertSub.Subscriber
                });
            }
            return vm;
        }

        public void SaveAlert(Guid? alertId, string alertName, string conditionString, string insertedBy, AlertType alertType, int? alertMins, List<AlertCondition> conds)
        {
            Guid alert = alertId ?? Guid.NewGuid();
            bool summaryAlert = alertType == AlertType.Summary;
            const string query = @"
--declare @alertId uniqueidentifier,@name varchar(100),@conditionString varchar(max),@insertedDate datetime,@insertedBy varchar(100)
insert into Alert
(
AlertID, 
Name, 
ConditionsString, 
InsertedDate, 
InsertedBy
)
values
(
ISNULL(@alertId,newid())
,@name
,@conditionString
,GETDATE()
,@insertedBy
)

insert into Portal_Alert(AlertID, IsSummaryAlert,IntervalMins)
  values (@alertId,@summaryAlert,@alertMins)

delete from [dbo].[AlertCondition]
where alertid = @alertId
";

            int result = _dbConnection.Execute(query, new
            {
                alertId = alert,
                name = alertName,
                conditionString,
                insertedBy = insertedBy,
                summaryAlert = summaryAlert,
                alertMins = alertMins

            }, null, _commandTimeoutSeconds);


            foreach (var condition in conds)
            {
                _dbConnection.Execute(@"insert into [dbo].[AlertCondition](AlertID, LeftSide, RightSide, Operator, InsertedDate) values(@AlertID, @LeftSide, @RightSide, @Operator, getdate())", new
                {
                    alertId = alert,
                    LeftSide = condition.Criteria.ToDbStringAnsi(),
                    RightSide = condition.Value.ToDbStringAnsi(),
                    Operator = condition.Operation.ToDbStringAnsi(),


                }, null, _commandTimeoutSeconds);
            }
        }

        public void DeleteAlert(Guid? alertId, string deletedBy)
        {
            const string query = @"
delete from alert
  where AlertID = @alertId

delete from Portal_Alert
  where AlertID = @alertId
";

            int result = _dbConnection.Execute(query, new
            {
                alertId = alertId

            }, null, _commandTimeoutSeconds);

        }


        public void SaveAlertSubscription(EsbAlertSubscription alertSubscription, string currentUser)
        {
            const string query = @"
insert into [dbo].[AlertSubscription]
(
AlertSubscriptionID,
AlertID, 
Active, 
Subscriber, 
IsGroup, 
CustomEmail, 
UseStartAndEndTime, 
InsertedDate, 
InsertedBy, 
ModifiedDate, 
ModifiedBy
)
values
(
newid(),
@alertId,
1,
@subscriber,
@isGroup,
@customEmail,
0,
getdate(),
@currentUser,
getdate(),
@currentUser
)
";

            int result = _dbConnection.Execute(query, new
            {
                alertId = alertSubscription.AlertId,
                subscriber = alertSubscription.Subscriber.ToDbStringAnsi(),
                isGroup = alertSubscription.IsGroup,
                customEmail = alertSubscription.CustomEmail,
                currentUser = currentUser.ToDbStringAnsi()

            }, null, _commandTimeoutSeconds);

        }

        public void ToggleAlertSubscriptionState(Guid alertSubscriptionId, bool state, string currentUser)
        {
            const string query = @"
update AlertSubscription
set Active = @currentState,ModifiedDate = getdate(),ModifiedBy = @currentUser
where AlertSubscriptionID = @alertSubscriptionId

";

            int result = _dbConnection.Execute(query, new
            {
                currentState = state,
                alertSubscriptionId = alertSubscriptionId,
                currentUser = currentUser.ToDbStringAnsi()

            }, null, _commandTimeoutSeconds);

        }

        public void DeleteAlertSubscription(Guid alertSubscriptionId)
        {
            const string query = @"
delete from AlertSubscription
where AlertSubscriptionID = @alertSubscriptionId

";

            int result = _dbConnection.Execute(query, new
            {
                alertSubscriptionId = alertSubscriptionId

            }, null, _commandTimeoutSeconds);

        }

        public List<EsbConfiguration> GetEsbConfiguration()
        {
            const string query = @"
SELECT [ConfigurationID]
      ,[Name]
      ,[Value]
      ,[Description]
      ,[ModifiedDate]
      ,[ModifiedBy]
  FROM [EsbExceptionDb].[dbo].[Configuration]
";

            return _dbConnection.Query<EsbConfiguration>(query).ToList();
        }

        public void UpdateEsbConfiguration(Guid configurationId, string value, string user)
        {
            const string query = @"
update Configuration
  set Value = @value,
  ModifiedBy = @user,
  ModifiedDate = GETDATE()
  where ConfigurationID = @configurationId

";

            int result = _dbConnection.Execute(query, new
            {
                configurationId = configurationId,
                value = value.ToDbStringAnsi(length: 256),
                user = user.ToDbStringAnsi()


            }, null, _commandTimeoutSeconds);

        }

        public AlertNotification GetAlertNotifications(int batchSize)
        {
            var vm = new AlertNotification();
            const string query = @"
DECLARE @Statement nvarchar(4000)
--,@BatchSize int = 500
,@batchId uniqueidentifier = newid()

insert into [dbo].[Batch](BatchID, StartDatetime)
values(@batchId,getdate())


SET @Statement = N'SELECT TOP ' + CONVERT(varchar, @BatchSize) + '
   f.[FaultID]
  ,f.[Application]
  ,f.[Description]
  ,f.[ErrorType]
  ,f.[FailureCategory]
  ,f.[FaultCode]
  ,f.[FaultDescription]
  ,f.[FaultSeverity]
  ,f.[Scope]
  ,f.[ServiceInstanceID]
  ,f.[ServiceName]
  ,f.[FaultGenerator]
  ,f.[MachineName]
  ,f.[DateTime]
  ,f.[ExceptionMessage]
  ,f.[ExceptionType]
  ,f.[ExceptionSource]
  ,f.[ExceptionTargetSite]
  ,f.[ExceptionStackTrace]
  ,f.[InnerExceptionMessage]
  ,f.[InsertedDate]	
FROM dbo.Fault f 
LEFT OUTER JOIN dbo.ProcessedFault pf ON pf.ProcessedFaultID = f.FaultID
LEFT OUTER JOIN [dbo].[Portal_FaultStatus] fs ON fs.FaultID = f.FaultID
WHERE 
		 pf.ProcessedFaultID IS NULL
and ISNULL(fs.[FaultStatusTypeId],1) = 1
'

CREATE TABLE #tempFaults(
    [FaultID] [uniqueidentifier] NOT NULL,
	[Application] [varchar](256) NOT NULL,
	[Description] [varchar](4096) NULL,
	[ErrorType] [varchar](100) NOT NULL,
	[FailureCategory] [varchar](256) NOT NULL,
	[FaultCode] [varchar](20) NOT NULL,
	[FaultDescription] [varchar](4096) NULL,
	[FaultSeverity] [int] NULL,
	[Scope] [varchar](256) NOT NULL,
	[ServiceInstanceID] [varchar](38) NOT NULL,
	[ServiceName] [varchar](256) NOT NULL,
	[FaultGenerator] [varchar](50) NULL,
	[MachineName] [varchar](256) NULL,
	[DateTime] [datetime] NULL,
	[ExceptionMessage] [varchar](4096) NOT NULL,
	[ExceptionType] [varchar](100) NOT NULL,
	[ExceptionSource] [varchar](256) NOT NULL,
	[ExceptionTargetSite] [varchar](256) NOT NULL,
	[ExceptionStackTrace] [varchar](4096) NOT NULL,
	[InnerExceptionMessage] [varchar](4096) NOT NULL,
	[InsertedDate] [datetime]
 )

 CREATE TABLE #tempFaults2(
    [FaultID] [uniqueidentifier] NOT NULL,
	[Application] [varchar](256) NOT NULL,
	[Description] [varchar](4096) NULL,
	[ErrorType] [varchar](100) NOT NULL,
	[FailureCategory] [varchar](256) NOT NULL,
	[FaultCode] [varchar](20) NOT NULL,
	[FaultDescription] [varchar](4096) NULL,
	[FaultSeverity] [int] NULL,
	[Scope] [varchar](256) NOT NULL,
	[ServiceInstanceID] [varchar](38) NOT NULL,
	[ServiceName] [varchar](256) NOT NULL,
	[FaultGenerator] [varchar](50) NULL,
	[MachineName] [varchar](256) NULL,
	[DateTime] [datetime] NULL,
	[ExceptionMessage] [varchar](4096) NOT NULL,
	[ExceptionType] [varchar](100) NOT NULL,
	[ExceptionSource] [varchar](256) NOT NULL,
	[ExceptionTargetSite] [varchar](256) NOT NULL,
	[ExceptionStackTrace] [varchar](4096) NOT NULL,
	[InnerExceptionMessage] [varchar](4096) NOT NULL,
	[InsertedDate] [datetime]
 )

insert into #tempFaults
EXECUTE sp_executesql @Statement

create table #tempFaultsToAlerts
(
faultid uniqueidentifier,
alertid uniqueidentifier
)

create table #tempAlerts
(
alertid uniqueidentifier,
ConditionsString varchar(max)

)
insert into #tempAlerts
--all valid alerts
select distinct a.AlertID,ConditionsString
from alert a
inner join AlertSubscription asu on a.AlertID = asu.AlertID and asu.Active = 1
left outer join Portal_Alert pa on a.AlertID = pa.AlertID
where ISNULL(pa.IsSummaryAlert,0) = 0

-----
Declare @alertId uniqueidentifier, @conditionsString varchar(max),@filterStatement nvarchar(4000)

DECLARE alert_cursor CURSOR FOR   
SELECT alertid, conditionsstring  
FROM #tempAlerts  
  
OPEN alert_cursor  
  
FETCH NEXT FROM alert_cursor   
INTO @alertId, @conditionsString  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	set @filterStatement = 'select * from #tempFaults where ' + @conditionsString

	--print @filterStatement
	insert into #tempFaults2
	EXECUTE sp_executesql @filterStatement
	

	insert into #tempFaultsToAlerts
	select distinct faultid,@alertId from #tempFaults2

	
    truncate table #tempFaults2

      
    FETCH NEXT FROM alert_cursor   
    INTO @alertId, @conditionsString   
END   
CLOSE alert_cursor;  
DEALLOCATE alert_cursor; 

--mark faults as processed
insert into ProcessedFault
select distinct faultid from #tempFaults


select @batchId as BatchId
select * from #tempFaultsToAlerts
select * from #tempFaults where faultid in (select distinct faultid from #tempFaultsToAlerts)

select a.AlertID, Name, ConditionsString,ia.[LastFired],ia.[IntervalMins],ISNULL(ia.[IsSummaryAlert],0) as IsSummaryAlert
from Alert a
left outer join [dbo].[Portal_Alert] ia (NOLOCK) on a.AlertID = ia.alertid and ISNULL(ia.[IsSummaryAlert],0) = 0

select a.AlertID, Name, ConditionsString,[Subscriber],[IsGroup],[CustomEmail],asu.AlertSubscriptionID
from Alert a
inner join [dbo].[AlertSubscription] asu on asu.AlertID = a.AlertID and asu.Active=1
where a.AlertID in (select distinct alertid from #tempFaultsToAlerts)


drop table #tempFaults
drop table #tempFaults2
drop table #tempFaultsToAlerts
drop table #tempAlerts
";

            using (var multi = _dbConnection.QueryMultiple(query, new
            {
                batchSize = batchSize
            },
             commandTimeout: _commandTimeoutSeconds))
            {
                vm.BatchId = multi.Read<Guid>().FirstOrDefault();
                vm.AlertFaults = multi.Read<AlertFault>().ToList();
                vm.FaultDetails = multi.Read<FaultDetail>().ToList();
                vm.Alerts = multi.Read<EsbAlert>().ToList();
                vm.AlertSubscriptions = multi.Read<EsbAlertSubscription>().ToList();
            }

            return vm;
        }

        public void InsertAlertEmail(List<AlertEmailNotification> emailNotifications)
        {
            const string statment = @"insert into [dbo].[Portal_AlertEmail](AlertID, [To], [Subject], Body, [Sent], BatchID,IsSummaryAlert) 
                                      values (@AlertID, @To, @Subject, @Body, @Sent, @BatchID,@IsSummaryAlert)";

            foreach (var emailNotification in emailNotifications)
            {
                _dbConnection.Execute(statment, emailNotification);
            }

        }

        public void UpdateAlertLastFired(List<Guid> alertIds, DateTime lastFiredTime)
        {
            string query = @"update [dbo].[Portal_Alert]
set [LastFired] = @lastFiredTime
where [AlertID] = @alertId

if @@ROWCOUNT = 0
insert into Portal_Alert(AlertId,lastfired) values (@alertId,@lastFiredTime)";
            foreach (var alertId in alertIds)
            {
                _dbConnection.Execute(query, new
                {
                    alertId = alertId,
                    lastFiredTime = lastFiredTime
                });
            }
        }

        public void UpdateAlertBatchComplete(Guid batchId, string errorMsg)
        {
            string query = @"update Batch
                      set [EndDatetime] = getdate() ,[ErrorMessage] = @errorMsg
                      where [BatchID] = @batchId";

            _dbConnection.Execute(query, new
            {
                batchId = batchId,
                errorMsg = errorMsg.ToDbStringAnsi(length: 500)
            });
        }

        public List<AlertEmailNotification> GetAlertEmails(int batchSize)
        {
            const string query = @"
SELECT TOP (@batchSize) [AlertEmailID]
      ,[AlertID]
      ,[To]
      ,[Subject]
      ,[Body]
      ,[Sent]
      ,[BatchID]
      ,[InsertedDate]
      ,[Error]
      ,IsSummaryAlert
  FROM [EsbExceptionDb].[dbo].[Portal_AlertEmail] (NOLOCK)
  where sent = 0
";

            var data = _dbConnection.Query<AlertEmailNotification>(query, new
            {
                BatchSize = batchSize
            }).ToList();
            return data;
        }

        public void UpdateAlertEmails(List<AlertEmailNotification> emailNotifications)
        {
            string query = @"update [dbo].[Portal_AlertEmail]
set [Sent] = @sent,
error = @error
where [AlertEmailID] = @alertEmailId";
            foreach (var emailNotif in emailNotifications)
            {
                _dbConnection.Execute(query, new
                {
                    sent = emailNotif.Sent,
                    error = emailNotif.Error,
                    alertEmailId = emailNotif.AlertEmailId
                });
            }
        }

        public AlertView GetAlert(Guid? alertId)
        {
            const string query = @"
                    select a.AlertID, Name,ISNULL(ia.IsSummaryAlert,0) as IsSummaryAlert,ia.IntervalMins
                    from alert a
                    left outer join Portal_Alert ia on a.AlertID = ia.alertid
                    where a.AlertID = @alertId

                    select 
                    AlertID, 
                    LeftSide as Criteria, 
                    RightSide as Value, 
                    Operator as Operation, 
                    InsertedDate
                    from [dbo].[AlertCondition]
                    where AlertID=@alertId
                    ";

            var vm = new AlertView();
            using (var multi = _dbConnection.QueryMultiple(query,
                new
                {
                    alertId = alertId,

                }, commandTimeout: _commandTimeoutSeconds))
            {
                vm.Alert = multi.Read<EsbAlert>().FirstOrDefault();
                vm.AlertConditions = multi.Read<AlertCondition>().ToList();
            }


            return vm;

        }

        public void UpdateAlert(Guid alertId, AlertType alertType, int? alertMins)
        {
            bool isSummary = alertType == AlertType.Summary;
            const string query = @"update Portal_Alert
                        set [IsSummaryAlert] = @isSummary,[IntervalMins] = @alertMins
                        where alertid = @alertid

                        if @@ROWCOUNT = 0
                        insert into Portal_Alert(AlertID, IsSummaryAlert, IntervalMins)
                        values(@alertId,@isSummary,@alertMins)";

            _dbConnection.Execute(query, new
            {
                alertId = alertId,
                isSummary = isSummary,
                alertMins = alertMins
            });
        }

        public AlertNotification GetAlertSummaryNotifications()
        {
            var vm = new AlertNotification();
            const string query = @"
CREATE TABLE #tempFaults2(
    AlertId uniqueidentifier NOT NULL,
	[Application] [varchar](256) NOT NULL,
	[ServiceName] [varchar](256) NOT NULL,
	[ExceptionType] [varchar](100) NOT NULL,
	Count int,
	MaxTime datetime,
	MinTime datetime
 )
 
create table #tempAlerts
(
alertid uniqueidentifier,
ConditionsString varchar(max),
lastfired datetime,
intervalmins smallint
)

insert into #tempAlerts
--all valid alerts
select distinct a.AlertID,ConditionsString,ISNULL(ia.lastfired,'2000-01-01') as lastfired,ia.intervalmins
from Alert a (NOLOCK)
inner join AlertSubscription asu (NOLOCK) on a.AlertID = asu.AlertID and asu.Active = 1
left outer join Portal_Alert ia (NOLOCK) on ia.alertid = a.AlertID
where ia.issummaryalert = 1
and DATEDIFF(minute,ISNULL(ia.lastfired,'2000-01-01'),getdate()) > ISNULL(ia.intervalmins,120)

-----
Declare @alertId uniqueidentifier, @conditionsString varchar(max),@filterStatement nvarchar(4000),@intervalmins smallint
,@lastfired datetime

DECLARE alert_cursor CURSOR FOR   
SELECT alertid, conditionsstring ,lastfired,intervalmins
FROM #tempAlerts  
  
OPEN alert_cursor  
  
FETCH NEXT FROM alert_cursor   
INTO @alertId, @conditionsString ,@lastfired,@intervalmins
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
	declare @now datetime = getdate()
	set @filterStatement = 'select distinct ''' + cast(@alertId as varchar(50)) + ''' ,Application,ServiceName,ExceptionType,count(*) as Count,max(datetime) as MaxTime,min(datetime) as MinTime from Fault (NOLOCK)
				where InsertedDate >= ''' + convert(varchar(25), @lastfired, 120)  
				+ ''' and InsertedDate < ''' + convert(varchar(25), @now, 120)  
				+ ''' and ' + @conditionsString +
				' group by Application,ExceptionType,ServiceName order by Count desc'

	print @filterStatement
	insert into #tempFaults2
	EXECUTE sp_executesql @filterStatement
	
	--insert into #tempFaultsToAlerts
	--select distinct faultid,@alertId from #tempFaults2
    
	update [dbo].[Portal_Alert]
	set [LastFired] = @now
	where [AlertID] = @alertId

    FETCH NEXT FROM alert_cursor   
    INTO @alertId, @conditionsString ,@lastfired,@intervalmins
END   
CLOSE alert_cursor;  
DEALLOCATE alert_cursor; 


select * from #tempFaults2

select a.AlertID, Name, ConditionsString,[Subscriber],[IsGroup],[CustomEmail],asu.AlertSubscriptionID
from Alert a
inner join [dbo].[AlertSubscription] asu on asu.AlertID = a.AlertID and asu.Active=1
where a.AlertID in (select distinct alertid from #tempFaults2)


drop table #tempFaults2
drop table #tempAlerts


";

            using (var multi = _dbConnection.QueryMultiple(query, null, commandTimeout: _commandTimeoutSeconds))
            {
                vm.AlertFaultSummaries = multi.Read<AlertFaultSummary>().ToList();
                vm.AlertSubscriptions = multi.Read<EsbAlertSubscription>().ToList();
            }

            return vm;
        }

        public void InsertFault(FaultDetail faultDetail, List<FaultMessage> faultMessages, List<FaultMessageContext> faultMessageContexts, List<FaultMessageData> faultMessageDatas, FaultHistory history)
        {
            const string faultQuery = @"
insert into [dbo].[Fault]
(
FaultID, 
NativeMessageID, 
ActivityID, 
Application, 
Description, 
ErrorType, 
FailureCategory, 
FaultCode, 
FaultDescription, FaultSeverity, Scope, ServiceInstanceID, ServiceName, FaultGenerator, 
MachineName, DateTime, ExceptionMessage, ExceptionType, ExceptionSource, ExceptionTargetSite, 
ExceptionStackTrace, InnerExceptionMessage, InsertMessagesFlag, InsertedDate
)
values
(
@FaultID, 
@NativeMessageID, 
@ActivityID, 
@Application, 
@Description, 
@ErrorType, 
@FailureCategory, 
@FaultCode, 
@FaultDescription, @FaultSeverity, @Scope, @ServiceInstanceID, @ServiceName, @FaultGenerator, 
@MachineName, @DateTime, @ExceptionMessage, @ExceptionType, @ExceptionSource, @ExceptionTargetSite, 
@ExceptionStackTrace, @InnerExceptionMessage, @InsertMessagesFlag, @InsertedDate

)
";

            _dbConnection.Execute(faultQuery, faultDetail, null, _commandTimeoutSeconds);

            foreach (var message in faultMessages)
            {
                const string query = @"
insert into Message
(
MessageID, NativeMessageID, FaultID, ContentType, MessageName, InterchangeID
)
values
(
@MessageID, 
@NativeMessageID, 
@FaultID, 
@ContentType, 
@MessageName, 
@InterchangeID

)
";

                _dbConnection.Execute(query, message, null, _commandTimeoutSeconds);
            }

            foreach (var message in faultMessageDatas)
            {
                const string query = @"
insert into MessageData
(MessageID, MessageData)
values (@MessageID, @MessageData)
";

                _dbConnection.Execute(query, message, null, _commandTimeoutSeconds);
            }

            foreach (var message in faultMessageContexts)
            {
                const string query = @"
insert into ContextProperty
(ContextPropertyID, MessageID, Name, Value, Type, InsertedDate)
values
(@ContextPropertyID, @MessageID, @Name, @Value, @Type, GETUTCDATE())
";

                _dbConnection.Execute(query, message, null, _commandTimeoutSeconds);
            }

            const string historyQuery = @"
insert into [dbo].[Portal_FaultHistory]
(FaultID, FaultStatusTypeId, UpdatedTime, UpdatedBy, UpdatedBatchId, Comment)
values(@FaultID, 1, getutcdate(), @UpdatedBy, NEWID(), @Comment)
";

            _dbConnection.Execute(historyQuery, history, null, _commandTimeoutSeconds);
        }
    }
}