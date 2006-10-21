/*Add the Mail to weblog parameters to subtext_config - GY*/
IF NOT EXISTS 
	(
		SELECT	* FROM [information_schema].[columns] 
		WHERE	table_name = 'subtext_Config' 
		AND		table_schema = '<dbUser,varchar,dbo>'
		AND		column_name = 'pop3User' 
		AND		column_name = 'pop3Pass'
		AND		column_name = 'pop3Server'
		AND		column_name = 'pop3StartTag'
		AND		column_name = 'pop3EndTag'
		AND		column_name = 'pop3SubjectPrefix'
		AND		column_name = 'pop3MTBEnable'
		AND		column_name = 'pop3DeleteOnlyProcessed'
		AND		column_name = 'pop3InlineAttachedPictures'
		AND		column_name = 'pop3HeightForThumbs'
	)
	
	ALTER TABLE [<dbUser,varchar,dbo>].[subtext_Config] WITH NOCHECK
	ADD [pop3User] [varchar](32) NULL
		,[pop3Pass] [varchar] (32) NULL
		,[pop3Server] [varchar] (56) NULL
		,[pop3StartTag] [varchar] (10) NULL
		,[pop3EndTag] [varchar] (10) NULL
		,[pop3SubjectPrefix] [nvarchar] (20) NULL
		,[pop3MTBEnable] [bit] NULL
		,[pop3DeleteOnlyProcessed] [bit] NULL
		,[pop3InlineAttachedPictures] [bit] NULL
		,[pop3HeightForThumbs] [int] NULL 

GO




/* Add tables to manage plugin configuration */


if not exists (select * from dbo.sysobjects where id = object_id(N'[<dbUser,varchar,dbo>].[subtext_PluginConfiguration]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [<dbUser,varchar,dbo>].[subtext_PluginConfiguration] (
		[id] [int] IDENTITY (1, 1) NOT NULL ,
		[PluginId] [int] NOT NULL ,
		[BlogId] [int] NOT NULL ,
		[Key] [nvarchar] (256) COLLATE Latin1_General_CI_AS NOT NULL ,
		[Value] [ntext] COLLATE Latin1_General_CI_AS NOT NULL 
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	
	ALTER TABLE [<dbUser,varchar,dbo>].[subtext_PluginConfiguration] WITH NOCHECK ADD 
	CONSTRAINT [PK_subtext_PluginConfiguration] PRIMARY KEY  CLUSTERED 
	(
		[id]
	)  ON [PRIMARY] 

	EXEC sp_tableoption  N'[<dbUser,varchar,dbo>].[subtext_PluginConfiguration]', 'text in row', '2048'
END
GO



if not exists (select * from dbo.sysobjects where id = object_id(N'[<dbUser,varchar,dbo>].[subtext_PluginEntryData]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [<dbUser,varchar,dbo>].[subtext_PluginEntryData] (
		[id] [int] IDENTITY (1, 1) NOT NULL ,
		[PluginId] [int] NOT NULL ,
		[BlogId] [int] NOT NULL ,
		[EntryId] [int] NOT NULL ,
		[Key] [nvarchar] (256) COLLATE Latin1_General_CI_AS NOT NULL ,
		[Value] [ntext] COLLATE Latin1_General_CI_AS NOT NULL 
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	
	ALTER TABLE [<dbUser,varchar,dbo>].[subtext_PluginEntryData] WITH NOCHECK ADD 
	CONSTRAINT [PK_subtext_PluginEntryData] PRIMARY KEY  CLUSTERED 
	(
		[id]
	)  ON [PRIMARY]
	
	EXEC sp_tableoption  N'[<dbUser,varchar,dbo>].[subtext_PluginEntryData]', 'text in row', '2048'
END
GO

