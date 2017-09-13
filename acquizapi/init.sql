
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

/****** Object:  Table [dbo].[quizuser]    Script Date: 2017-08-24 10:36:13 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[quizuser](
	[userid] [nvarchar](50) NOT NULL,
	[displayas] [nvarchar](50) NOT NULL,
	[others] [nvarchar](50) NULL,
 CONSTRAINT [PK_quizuser] PRIMARY KEY CLUSTERED 
(
	[userid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- Following parts are updated at 2017.9.2

/****** Object:  Table [dbo].[permuser]    Script Date: 2017-09-02 9:15:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[permuser](
	[userid] [nvarchar](50) NOT NULL,
	[monitor] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_permuser] PRIMARY KEY CLUSTERED 
(
	[userid] ASC,
	[monitor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[awardplan]    Script Date: 2017-09-02 9:43:08 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[awardplan](
	[planid] [int] IDENTITY(1,1) NOT NULL,
	[tgtuser] [nvarchar](50) NOT NULL,
	[createdby] [nvarchar](50) NULL,
	[validfrom] [date] NOT NULL,
	[validto] [date] NOT NULL,
	[quiztype] [smallint] NOT NULL,
	[minscore] [int] NULL,
	[minavgtime] [int] NULL,
	[award] [int] NOT NULL,
 CONSTRAINT [PK_awardplan] PRIMARY KEY CLUSTERED 
(
	[planid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


/****** Object:  Table [dbo].[useraward]    Script Date: 2017-09-02 9:16:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[useraward](
	[aid] [int] IDENTITY(1,1) NOT NULL,
	[userid] [nvarchar](50) NOT NULL,
	[adate] [date] NOT NULL,
	[award] [int] NOT NULL,
	[planid] [int] NULL,
	[qid] [int] NULL,
	[used] [nvarchar](50) NULL,
 CONSTRAINT [PK_useraward] PRIMARY KEY CLUSTERED 
(
	[aid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- Following parts are updated at 2017.9.3

/****** Object:  View [dbo].[v_permuser]    Script Date: 2017-09-03 4:56:08 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_permuser]
AS
SELECT DISTINCT userid, monitor
FROM            (SELECT        userid, monitor
                          FROM            dbo.permuser
                          UNION ALL
                          SELECT        userid, userid AS monitor
                          FROM            dbo.permuser AS permuser_1) AS derivedtbl_1
GO

/****** Object:  View [dbo].[v_useraward]    Script Date: 2017-09-03 8:34:51 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_useraward]
AS
SELECT        dbo.useraward.aid, dbo.useraward.userid, dbo.useraward.adate, dbo.useraward.award, dbo.useraward.planid, dbo.awardplan.quiztype, dbo.useraward.qid, dbo.useraward.used
FROM            dbo.useraward INNER JOIN
                         dbo.awardplan ON dbo.useraward.planid = dbo.awardplan.planid
GO

-- Following parts are updated at 2017.9.11
/* Alter table by adding columns */
ALTER TABLE [dbo].[quizuser]
ADD 
	[award] [nvarchar](5) NULL,
	[awardplan] [nvarchar](5) NULL,
	[deletionflag] [bit] NULL
GO

-- Following parts are updated at 2017.9.11
/* Alter table by adding columns */
ALTER TABLE [dbo].[useraward]
ADD 
	[publish] bit NULL
GO

-- Rebuild view V_USERAWARD
DROP VIEW [dbo].[v_useraward]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_useraward]
AS
SELECT  taba.[aid], taba.[userid], taba.[adate], taba.[award], taba.[planid], tabb.[quiztype], taba.[qid], taba.[used], taba.[publish]
FROM    [dbo].[useraward] taba LEFT OUTER JOIN [dbo].[awardplan] tabb ON taba.[planid] = tabb.[planid]
GO
