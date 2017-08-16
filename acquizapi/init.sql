﻿/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2016 (13.0.1601)
    Source Database Engine Edition : Microsoft SQL Server Express Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Standard Edition
    Target Database Engine Type : Standalone SQL Server
*/

USE [master]
GO

/****** Object:  Database [acquizdb]    Script Date: 2017-08-15 6:13:38 PM ******/
CREATE DATABASE [acquizdb]
GO

ALTER DATABASE [acquizdb] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [acquizdb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [acquizdb] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [acquizdb] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [acquizdb] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [acquizdb] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [acquizdb] SET ARITHABORT OFF 
GO
ALTER DATABASE [acquizdb] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [acquizdb] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [acquizdb] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [acquizdb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [acquizdb] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [acquizdb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [acquizdb] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [acquizdb] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [acquizdb] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [acquizdb] SET  DISABLE_BROKER 
GO
ALTER DATABASE [acquizdb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [acquizdb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [acquizdb] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [acquizdb] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [acquizdb] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [acquizdb] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [acquizdb] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [acquizdb] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [acquizdb] SET  MULTI_USER 
GO
ALTER DATABASE [acquizdb] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [acquizdb] SET DB_CHAINING OFF 
GO
ALTER DATABASE [acquizdb] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [acquizdb] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [acquizdb] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [acquizdb] SET QUERY_STORE = OFF
GO
USE [acquizdb]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
USE [acquizdb]
GO


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

