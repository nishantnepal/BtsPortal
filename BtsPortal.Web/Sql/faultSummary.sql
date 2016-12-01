declare @startDateTime datetime
declare @endDateTime datetime

select application,UnResolved,Ignored,Resubmitted,Flagged,LastFaultOccurredDate
from
(
select f.application,
Isnull(fst.[Name],'UnResolved') as FaultStatus
,f.FaultID
,md.LastFaultOccurredDate
from Fault f (NOLOCK)
left outer join [dbo].[IA_FaultStatus] fs (NOLOCK) on f.FaultID = fs.FaultId
left outer join [dbo].[IA_FaultStatusType] fst (NOLOCK) on fs.FaultStatusTypeID = fst.Id
left outer join
(
	select f.application
	, max([DateTime]) as LastFaultOccurredDate
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
( UnResolved,Ignored,Resubmitted,Flagged )
) AS pvt
ORDER BY application

select ErrorType,UnResolved,Ignored,Resubmitted,Flagged,LastFaultOccurredDate
from
(
select f.ErrorType,
Isnull(fst.[Name],'UnResolved') as FaultStatus
,f.FaultID
,md.LastFaultOccurredDate
from Fault f (NOLOCK)
left outer join [dbo].[IA_FaultStatus] fs (NOLOCK) on f.FaultID = fs.FaultId
left outer join [dbo].[IA_FaultStatusType] fst (NOLOCK) on fs.FaultStatusTypeID = fst.Id
left outer join
(
	select f.ErrorType
	, max([DateTime]) as LastFaultOccurredDate
	from Fault f (NOLOCK)
	where DateTime between ISNULL(@startDateTime,'2000-01-01') and ISNULL(@endDateTime,'2099-01-01')
	group by f.ErrorType
) as md on f.ErrorType = md.ErrorType
where DateTime between ISNULL(@startDateTime,'2000-01-01') and ISNULL(@endDateTime,'2099-01-01')
) p
PIVOT
(
COUNT (faultid)
FOR FaultStatus IN
( UnResolved,Ignored,Resubmitted,Flagged )
) AS pvt
ORDER BY ErrorType