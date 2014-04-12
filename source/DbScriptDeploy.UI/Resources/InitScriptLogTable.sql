DECLARE @tblExists int;
SELECT @tblExists = COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'ScriptLog';

IF @tblExists = 0
BEGIN

	CREATE TABLE [dbo].[ScriptLog](
		[Id] [uniqueidentifier] NOT NULL,
		[Name] [varchar](500) NOT NULL,
		[ScriptText] [varchar](max) NOT NULL,
		[CreatedOn] [datetime] NOT NULL,
		[CreatedUser] [varchar](200) NULL,
		[CreatedAccount] [varchar](200) NULL,
		[Archived] bit NOT NULL
	 CONSTRAINT [PK_ScriptLog] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

	ALTER TABLE [dbo].[ScriptLog] ADD  CONSTRAINT [DF_ScriptLog_Id]  DEFAULT (newid()) FOR [Id];

	ALTER TABLE [dbo].[ScriptLog] ADD  CONSTRAINT [DF_ScriptLog_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn];
	
	ALTER TABLE [dbo].[ScriptLog] ADD  CONSTRAINT [DF_ScriptLog_Archived]  DEFAULT (0) FOR [Archived];
END


