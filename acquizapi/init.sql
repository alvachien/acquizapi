
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

