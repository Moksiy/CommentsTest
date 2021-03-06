USE [BlogDB]
GO
/****** Object:  StoredProcedure [dbo].[GetFormatedComments]    Script Date: 25.08.2020 22:37:08 ******/
DROP PROCEDURE [dbo].[GetFormatedComments]
GO
/****** Object:  StoredProcedure [dbo].[GetComments]    Script Date: 25.08.2020 22:37:08 ******/
DROP PROCEDURE [dbo].[GetComments]
GO
/****** Object:  StoredProcedure [dbo].[GetArticleID]    Script Date: 25.08.2020 22:37:08 ******/
DROP PROCEDURE [dbo].[GetArticleID]
GO
ALTER TABLE [dbo].[Comments] DROP CONSTRAINT [FK_Comments_Users]
GO
ALTER TABLE [dbo].[Comments] DROP CONSTRAINT [FK_Comments_Comments]
GO
ALTER TABLE [dbo].[Comments] DROP CONSTRAINT [FK_Comments_Articles]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 25.08.2020 22:37:08 ******/
DROP TABLE [dbo].[Users]
GO
/****** Object:  Table [dbo].[Comments]    Script Date: 25.08.2020 22:37:08 ******/
DROP TABLE [dbo].[Comments]
GO
/****** Object:  Table [dbo].[Articles]    Script Date: 25.08.2020 22:37:08 ******/
DROP TABLE [dbo].[Articles]
GO
/****** Object:  Table [dbo].[Articles]    Script Date: 25.08.2020 22:37:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Articles](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](256) NOT NULL,
	[Text] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Articles] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Comments]    Script Date: 25.08.2020 22:37:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Comments](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Text] [nvarchar](max) NOT NULL,
	[UserID] [int] NOT NULL,
	[ArticleID] [int] NOT NULL,
	[ParentID] [int] NULL,
 CONSTRAINT [PK_Comments] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 25.08.2020 22:37:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD  CONSTRAINT [FK_Comments_Articles] FOREIGN KEY([ArticleID])
REFERENCES [dbo].[Articles] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Comments] CHECK CONSTRAINT [FK_Comments_Articles]
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD  CONSTRAINT [FK_Comments_Comments] FOREIGN KEY([ParentID])
REFERENCES [dbo].[Comments] ([ID])
GO
ALTER TABLE [dbo].[Comments] CHECK CONSTRAINT [FK_Comments_Comments]
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD  CONSTRAINT [FK_Comments_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Comments] CHECK CONSTRAINT [FK_Comments_Users]
GO
/****** Object:  StoredProcedure [dbo].[GetArticleID]    Script Date: 25.08.2020 22:37:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[GetArticleID] 
(
	@id INT
)
AS
BEGIN
SELECT        dbo.Articles.ID
                         FROM            dbo.Articles INNER JOIN
                         dbo.Comments ON dbo.Articles.ID = dbo.Comments.ArticleID
						 WHERE Comments.ID = @id
END
GO
/****** Object:  StoredProcedure [dbo].[GetComments]    Script Date: 25.08.2020 22:37:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetComments](@parentID INT)
AS
BEGIN
WITH tree (id, [Text], UserID, ArticleID, ParentID, "level", "path")
AS (
    SELECT T1.id, T1.[Text], T1.UserID, T1.ArticleID, T1.ParentID, 0, cast('' as varchar)
    FROM Comments T1
	WHERE T1.ID = @parentID

    UNION ALL

    SELECT T2.id, T2.[Text], T2.UserID, T2.ArticleID, T2.ParentID, T."level"+1, cast(T."path" +'.' + cast(T2.id as varchar) as varchar)
    FROM
        Comments T2 INNER JOIN tree T ON T.id = T2.ParentID
)
SELECT T.id, T.[Text], T.UserID, T.ArticleID, T.ParentID, level
FROM tree T
ORDER BY T."path"
END
GO
/****** Object:  StoredProcedure [dbo].[GetFormatedComments]    Script Date: 25.08.2020 22:37:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[GetFormatedComments]
(
@articleID INT
)
AS
BEGIN
WITH Tree(id, [Text], UserID, ArticleID, ParentID, "level", "path")
AS
(
	SELECT T1.id, T1.[Text], T1.UserID, T1.ArticleID, T1.ParentID, 0, cast(ID as varchar)
	FROM Comments T1
	WHERE T1.ParentID IS NULL AND T1.ArticleID = @articleID
	
	UNION ALL

	SELECT T2.id, T2.[Text], T2.UserID, T2.ArticleID, T2.ParentID, T."level"+1, cast(T."path" +'.' + cast(T2.id as varchar) as varchar)
    FROM
        Comments T2 JOIN tree T ON T.id = T2.ParentID
)

SELECT T.id, T.[Text], level, T.ParentID, (SELECT Text FROM Comments WHERE ID = T.ParentID) as [Text], a.ID, a.Title, a.[Text], u.ID, u.Name 
FROM tree T
      INNER JOIN Users u ON u.ID = T.UserID 
      INNER JOIN Articles a ON a.ID = T.ArticleID 
ORDER BY T."path"
END
/*************При поддержке высших сил, терпения и деда*************/
GO
