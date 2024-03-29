USE [master]
GO
/****** Object:  Database [Database Name]    Script Date: 27-06-2016 17:46:38 ******/
CREATE DATABASE [CustomDatabase]
GO
USE [CustomDatabase]
GO
/****** Object:  UserDefinedTableType [dbo].[CommentType]    Script Date: 27-06-2016 17:46:39 ******/
CREATE TYPE [dbo].[CommentType] AS TABLE(
	[id] [int] NULL,
	[BlogID] [varchar](150) NULL,
	[FullName] [varchar](500) NULL,
	[Email] [varchar](500) NULL,
	[Comment] [varchar](max) NULL,
	[IsSync] [bit] NULL,
	[CommentDateTime] [datetime] NULL,
	[UpdateDateTime] [datetime] NULL
)
GO
/****** Object:  Table [dbo].[BlogComments]    Script Date: 27-06-2016 17:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BlogComments](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[BlogID] [varchar](150) NULL,
	[FullName] [varchar](500) NULL,
	[Email] [varchar](500) NULL,
	[Comment] [varchar](max) NULL,
	[IsSync] [bit] NULL CONSTRAINT [DF_BlogComments_IsSync]  DEFAULT ((0)),
	[CommentDateTime] [datetime] NULL CONSTRAINT [DF_BlogComments_CommentDateTime]  DEFAULT (getdate()),
	[UpdateDateTime] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[usp_AddComment]    Script Date: 27-06-2016 17:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddComment]( @FullName Varchar(250),@Email varchar(250),@Comment varchar(max),@BLogID varchar(250))
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
 SET NOCOUNT ON;

 Insert into BlogComments(BlogID,FullName,Email,Comment) values (@BLogID,@FullName,@Email,@Comment)

END


GO
/****** Object:  StoredProcedure [dbo].[usp_GetUnProcessedComments]    Script Date: 27-06-2016 17:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetUnProcessedComments] 
	 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * from BlogComments where IsSync=0
END


GO
/****** Object:  StoredProcedure [dbo].[usp_UpdateProcessedResults]    Script Date: 27-06-2016 17:46:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_UpdateProcessedResults] @commentItems CommentType ReadOnly

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	update b set b.IsSync=1, b.UpdateDateTime=GETDATE()
	from @commentItems a join BlogComments b on a.id=b.id
	 
 
END

GO
