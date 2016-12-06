USE [EsbExceptionDb]
GO

IF OBJECT_ID('dbo.Portal_FaultHistory', 'U') IS NOT NULL 
  DROP TABLE dbo.Portal_FaultHistory; 

IF OBJECT_ID('dbo.Portal_FaultStatus', 'U') IS NOT NULL 
  DROP TABLE dbo.Portal_FaultStatus; 

 IF OBJECT_ID('dbo.Portal_FaultStatusType', 'U') IS NOT NULL 
  DROP TABLE dbo.Portal_FaultStatusType; 

 IF OBJECT_ID('dbo.Portal_Alert', 'U') IS NOT NULL 
  DROP TABLE dbo.Portal_Alert; 

IF OBJECT_ID('dbo.Portal_AlertEmail', 'U') IS NOT NULL 
  DROP TABLE dbo.Portal_AlertEmail; 

  

/****** Object:  Table [dbo].[Portal_FaultHistory]    Script Date: 11/17/2016 8:13:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Portal_FaultHistory](
	[FaultID] [uniqueidentifier] NOT NULL,
	[FaultStatusTypeId] [smallint] NOT NULL,
	[UpdatedTime] [datetime] NULL,
	[UpdatedBy] [varchar](50) NULL,
	[UpdatedBatchId] [uniqueidentifier] NULL,
	[Comment] [varchar](500) NULL
) ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX [NCI_Portal_FaultHistory_FaultID] ON [dbo].[Portal_FaultHistory]
(
	[FaultID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Portal_FaultStatus]    Script Date: 11/17/2016 8:13:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Portal_FaultStatus](
	[FaultID] [uniqueidentifier] NOT NULL,
	[FaultStatusTypeId] [smallint] NOT NULL,
 CONSTRAINT [PK_Portal_FaultStatus] PRIMARY KEY CLUSTERED 
(
	[FaultID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Portal_FaultStatusType]    Script Date: 11/17/2016 8:13:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Portal_FaultStatusType](
	[Id] [smallint] NOT NULL,
	[Name] [varchar](50) NULL,
 CONSTRAINT [PK_Portal_FaultStatusType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

insert into [dbo].[Portal_FaultStatusType](id,name)
values(1,'UnResolved')
insert into [dbo].[Portal_FaultStatusType](id,name)
values(2,'Resolved')
insert into [dbo].[Portal_FaultStatusType](id,name)
values(3,'Resubmitted')
insert into [dbo].[Portal_FaultStatusType](id,name)
values(4,'Flagged')


CREATE TABLE [dbo].[Portal_Alert](
	[AlertID] [uniqueidentifier] NOT NULL,
	[IsSummaryAlert] [bit] NULL,
	[LastFired] [datetime] NULL,
	[IntervalMins] [smallint] NULL,
 CONSTRAINT [PK_Portal_Alert] PRIMARY KEY CLUSTERED 
(
	[AlertID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[Portal_AlertEmail](
	[AlertEmailID] [uniqueidentifier] NOT NULL,
	[AlertID] [uniqueidentifier] NOT NULL,
	[To] [varchar](256) NULL,
	[Subject] [varchar](1024) NOT NULL,
	[Body] [varchar](max) NOT NULL,
	[Sent] [bit] NOT NULL,
	[BatchID] [uniqueidentifier] NOT NULL,
	[InsertedDate] [datetime] NOT NULL,
	[Error] [varchar](500) NULL,
	[IsSummaryAlert] [bit] ,
 CONSTRAINT [PK_Portal_AlertEmail] PRIMARY KEY CLUSTERED 
(
	[AlertEmailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[Portal_AlertEmail] ADD  CONSTRAINT [DF_Portal_AlertEmail_AlertEmailID]  DEFAULT (newid()) FOR [AlertEmailID]
GO

ALTER TABLE [dbo].[Portal_AlertEmail] ADD  CONSTRAINT [DF_Portal_AlertEmail_Sent]  DEFAULT ((0)) FOR [Sent]
GO

ALTER TABLE [dbo].[Portal_AlertEmail] ADD  CONSTRAINT [DF_Portal_AlertEmail_InsertedDate]  DEFAULT (getdate()) FOR [InsertedDate]
GO



--updates to existing
update [dbo].[Configuration]
  set [Value] = 500
  where Name = 'QueueBatchSize'

update [dbo].[Configuration]
  set [Value] = 500
  where Name = 'EmailBatchSize'

 if not exists (select 1 from Configuration where name='QueueSummaryInterval')
insert into [Configuration]([Name]  ,[Value],[Description])
  values ('QueueSummaryInterval','60000','Summary queue process sleep in milliseconds')

  if not exists (select 1 from Configuration where name='XsltSummaryPath')
  insert into [Configuration]([Name]  ,[Value],[Description])
  values ('XsltSummaryPath','C:\temp','Absolute path for summary XSLT file which is used to generate html email body')

  if not exists (select 1 from Configuration where name='IsSummaryQueueEnabled')
  insert into [Configuration]([Name]  ,[Value],[Description])
  values ('IsSummaryQueueEnabled','false','Enable or disable summary queue process')

  if not exists (select 1 from Configuration where name='SmtpPort')
  insert into [Configuration]([Name]  ,[Value],[Description])
  values ('SmtpPort','','[Optional]Smtp server port')

  if not exists (select 1 from Configuration where name='SmtpCredentialsUserName')
  insert into [Configuration]([Name]  ,[Value],[Description])
  values ('SmtpCredentialsUserName','','[Optional]Username for connecting to the smtp server')

  if not exists (select 1 from Configuration where name='SmtpCredentialsPassword')
  insert into [Configuration]([Name]  ,[Value],[Description])
  values ('SmtpCredentialsPassword','','[Optional]Password for connecting to the smtp server')

  if not exists (select 1 from Configuration where name='SmtpEnableSsl')
  insert into [Configuration]([Name]  ,[Value],[Description])
  values ('SmtpEnableSsl','false','[Optional]Use ssl for connecting to the smtp server')

  if not exists (select 1 from Configuration where name='BtsPortalRootPath')
  insert into [Configuration]([Name]  ,[Value],[Description])
  values ('BtsPortalRootPath','http://localhost/btsportal','Root path for the bts portal. Used for links in alert emails.')