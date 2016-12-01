 if db_id('BtsPortal') is null
	Create Database BtsPortal

go

Use btsportal
go

IF OBJECT_ID('dbo.BamActivityViewParameter', 'U') IS NOT NULL 
  DROP TABLE dbo.BamActivityViewParameter; 

IF OBJECT_ID('dbo.BamActivityView', 'U') IS NOT NULL 
  DROP TABLE dbo.BamActivityView; 

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BamActivityView](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ActivityName] [varchar](50) NULL,
	[ViewName] [varchar](50) NULL,
	[IsActive] [bit] NULL,
	[SqlToExecute] [varchar](max) NULL,
	[SqlNoOfRows] [varchar](max) NULL,
	[SqlOrderBy] [varchar](250) NULL,
	[NoOfRowsPerPage] [smallint] NULL,
	[FilterParameters] [varchar](max) NULL,
	[InsertedBy] [varchar](50) NULL,
	[InsertedDate] [datetime] NULL,
	[LastUpdatedBy] [varchar](50) NULL,
	[LastUpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_BamActivityView] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_BamActivityView_Activity_View_Names] UNIQUE NONCLUSTERED 
(
	[ActivityName] ASC,
	[ViewName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BamActivityViewParameter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BamActivityViewId] [int] NULL,
	[Name] [varchar](50) NULL,
	[DisplayName] [varchar](50) NULL,
	[Type] [varchar](50) NULL,
 CONSTRAINT [PK_BamActivityViewParameter] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[BamActivityViewParameter]  WITH CHECK ADD  CONSTRAINT [FK_BamActivityViewParameter_BamActivityView] FOREIGN KEY([BamActivityViewId])
REFERENCES [dbo].[BamActivityView] ([Id])
GO
ALTER TABLE [dbo].[BamActivityViewParameter] CHECK CONSTRAINT [FK_BamActivityViewParameter_BamActivityView]
GO