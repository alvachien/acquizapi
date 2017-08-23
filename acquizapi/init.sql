
/****** Object:  Table [dbo].[quizfaillog]    Script Date: 2017-08-16 2:48:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[quizfaillog](
	[quizid] [int] NOT NULL,
	[failidx] [int] NOT NULL,
	[expected] [nvarchar](50) NOT NULL,
	[inputted] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_quizfaillog] PRIMARY KEY CLUSTERED 
(
	[quizid] ASC,
	[failidx] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[quiz]    Script Date: 2017-08-16 2:48:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[quiz](
	[quizid] [int] IDENTITY(1,1) NOT NULL,
	[quiztype] [smallint] NOT NULL,
	[basicinfo] [nvarchar] (50) NOT NULL,
	[attenduser] [nvarchar](50) NOT NULL,
	[submitdate] [datetime] NOT NULL,
 CONSTRAINT [PK_quiz] PRIMARY KEY CLUSTERED 
(
	[quizid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  View [dbo].[v_quizfailure]    Script Date: 2017-08-16 2:48:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[v_quizfailure]
AS
SELECT        dbo.quizfaillog.quizid, dbo.quiz.quiztype, dbo.quiz.submitdate, dbo.quizfaillog.failidx, dbo.quiz.attenduser, dbo.quizfaillog.expected, dbo.quizfaillog.inputted
FROM            dbo.quiz INNER JOIN
                         dbo.quizfaillog ON dbo.quiz.quizid = dbo.quizfaillog.quizid
GO

/****** Object:  Table [dbo].[quizsection]    Script Date: 2017-08-16 2:48:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[quizsection](
	[quizid] [int] NOT NULL,
	[section] [int] NOT NULL,
	[timespent] [int] NOT NULL,
	[totalitems] [int] NOT NULL,
	[faileditems] [int] NOT NULL,
 CONSTRAINT [PK_quizsection] PRIMARY KEY CLUSTERED 
(
	[quizid] ASC,
	[section] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[quiz] ADD  CONSTRAINT [DF_quiz_submitdate]  DEFAULT (getdate()) FOR [submitdate]
GO
ALTER TABLE [dbo].[quizfaillog]  WITH CHECK ADD  CONSTRAINT [FK_quizfaillog_quiz] FOREIGN KEY([quizid])
REFERENCES [dbo].[quiz] ([quizid])
ON UPDATE CASCADE
ON DELETE CASCADE
GO


ALTER TABLE [dbo].[quizfaillog] CHECK CONSTRAINT [FK_quizfaillog_quiz]
GO
ALTER TABLE [dbo].[quizsection]  WITH CHECK ADD  CONSTRAINT [FK_quizsection_quiz] FOREIGN KEY([quizid])
REFERENCES [dbo].[quiz] ([quizid])
ON UPDATE CASCADE
ON DELETE CASCADE
GO


ALTER TABLE [dbo].[quizsection] CHECK CONSTRAINT [FK_quizsection_quiz]
GO

-- Following parts are updated at 2017.8.23
/****** Object:  View [dbo].[v_quizamountbydate]    Script Date: 2017-08-23 5:57:46 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_quizamountbydate]
AS
SELECT        CAST(submitdate AS DATE) as submitdate, attenduser, count( * ) as quizamount
FROM          quiz
GROUP BY	  CAST(submitdate AS DATE), attenduser
GO

/****** Object:  View [dbo].[v_quizamountbytype]    Script Date: 2017-08-23 5:58:21 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_quizamountbytype]
AS
SELECT  quiztype, attenduser, count( * ) as quizamount
FROM    quiz
GROUP BY quiztype, attenduser
GO

/****** Object:  View [dbo].[v_quizitemamountbydate]    Script Date: 2017-08-23 5:58:45 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_quizitemamountbydate]
AS
SELECT        CAST(dbo.quiz.submitdate AS DATE) AS submitdate, dbo.quiz.attenduser, SUM(dbo.quizsection.totalitems) AS totalitems, SUM(dbo.quizsection.faileditems) AS faileditems
FROM            dbo.quiz LEFT OUTER JOIN
                         dbo.quizsection ON dbo.quiz.quizid = dbo.quizsection.quizid
GROUP BY CAST(dbo.quiz.submitdate AS DATE), dbo.quiz.attenduser
GO

/****** Object:  View [dbo].[v_quizitemamountbytype]    Script Date: 2017-08-23 5:59:17 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_quizitemamountbytype]
AS
SELECT        dbo.quiz.quiztype, dbo.quiz.attenduser, SUM(dbo.quizsection.totalitems) AS totalitems, SUM(dbo.quizsection.faileditems) AS faileditems
FROM            dbo.quiz LEFT OUTER JOIN
                         dbo.quizsection ON dbo.quiz.quizid = dbo.quizsection.quizid
GROUP BY dbo.quiz.quiztype, dbo.quiz.attenduser
GO
