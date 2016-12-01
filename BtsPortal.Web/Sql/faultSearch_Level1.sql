/*

 Severity Date Code Category Application Error Type Scope Service Name Fault Generator Machine Name 
 

*/

--select
--FaultID, 
--NativeMessageID, 
--ActivityID, 
--Application, 
--Description, 
--ErrorType, 
--FailureCategory, 
--FaultCode, 
--FaultDescription, 
--FaultSeverity, 
--Scope, 
--ServiceInstanceID, 
--ServiceName, 
--FaultGenerator, 
--MachineName, 
--DateTime, 
--ExceptionMessage, 
--ExceptionType, 
--ExceptionSource, 
--ExceptionTargetSite, 
--ExceptionStackTrace, 
--InnerExceptionMessage, 
--InsertMessagesFlag, 
--InsertedDate
--from Fault f (NOLOCK)
--inner join IA_FaultStatus fs (NOLOCK) on f.FaultID = fs.FaultID

Declare @application varchar(100)
Declare @fromDateTime datetime,
	@toDateTime datetime,
	@faultStatusTypeId int,
	@errorType varchar(100),
	@faultCode varchar(100),
	@failureCategory varchar(100),
	@failureScope varchar(100),
	@faultId varchar(50),
	@message varchar(100)

--set @faultId='14E37C53-69AF-40EC-83BE-C10E46DEB536'
set @fromDateTime = '08/01/2016'
set @toDateTime = GETDATE()
set @application = 'UCare.HIPAA_837'

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
--ISNULL(fst.Name,'UnResolved') as Status
ISNULL(fs.FaultStatusTypeId,1) as Status,
f.InsertMessagesFlag
from Fault f (NOLOCK)
left outer join IA_FaultStatus fs (NOLOCK) on f.FaultID = fs.FaultID
--left outer join [dbo].[IA_FaultStatusType] fst (NOLOCK) on fs.FaultStatusTypeId=fst.Id
left outer join Message m (NOLOCK) on f.FaultID = m.FaultID
left outer join MessageData md (NOLOCK) on m.MessageID = md.MessageID
where isnull(f.Application,'') = ISNULL(@application,f.Application)
and f.DateTime between ISNULL(@fromDateTime,'2000-01-01') and ISNULL(@toDateTime,'2099-01-01')
and ISNULL(fs.FaultStatusTypeId,1) = ISNULL(@faultStatusTypeId,ISNULL(fs.FaultStatusTypeId,1))
and isnull(f.FaultID,'') = ISNULL(@faultId,f.FaultID)
and isnull(f.ErrorType,'')  like '%' + ISNULL(@errorType,f.ErrorType) + '%'
and isnull(f.FaultCode,'')  like '%' + ISNULL(@faultCode,f.FaultCode) + '%'
and isnull(md.MessageData,'')  like '%' + ISNULL(@message,md.MessageData) + '%'
and isnull(f.FailureCategory,'')  like '%' + ISNULL(@failureCategory,f.FailureCategory) + '%'
and isnull(f.Scope,'')  like '%' + ISNULL(@failureScope,f.Scope) + '%'
order by f.DateTime desc
--OFFSET  (@pageNum -1) * @pageSize ROWS 
--FETCH NEXT @pageSize ROWS ONLY ;

--total rows
select count(distinct f.FaultID) as TotalRows
from Fault f (NOLOCK) 
left outer join IA_FaultStatus fs (NOLOCK) on f.FaultID = fs.FaultID
left outer join [dbo].[IA_FaultStatusType] fst (NOLOCK) on fs.FaultStatusTypeId=fst.Id
left outer join Message m (NOLOCK) on f.FaultID = m.FaultID
left outer join MessageData md (NOLOCK) on m.MessageID = md.MessageID
where isnull(f.Application,'') = ISNULL(@application,f.Application)
and f.DateTime between ISNULL(@fromDateTime,'2000-01-01') and ISNULL(@toDateTime,'2099-01-01')
and ISNULL(fs.FaultStatusTypeId,1) = ISNULL(@faultStatusTypeId,ISNULL(fs.FaultStatusTypeId,1))
and isnull(f.FaultID,'') = ISNULL(@faultId,f.FaultID)
and isnull(f.ErrorType,'')  like '%' + ISNULL(@errorType,f.ErrorType) + '%'
and isnull(f.FaultCode,'')  like '%' + ISNULL(@faultCode,f.FaultCode) + '%'
and isnull(md.MessageData,'')  like '%' + ISNULL(@message,md.MessageData) + '%'
and isnull(f.FailureCategory,'')  like '%' + ISNULL(@failureCategory,f.FailureCategory) + '%'
and isnull(f.Scope,'')  like '%' + ISNULL(@failureScope,f.Scope) + '%'