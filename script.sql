USE [FileProcessingDB]
GO
ALTER TABLE [dbo].[FileProcessingRecords] DROP CONSTRAINT [FK_FileProcessingRecords_Status]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 23-10-2024 12:47:59 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
DROP TABLE [dbo].[Users]
GO
/****** Object:  Table [dbo].[Status]    Script Date: 23-10-2024 12:47:59 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Status]') AND type in (N'U'))
DROP TABLE [dbo].[Status]
GO
/****** Object:  Table [dbo].[FileProcessingRecords]    Script Date: 23-10-2024 12:47:59 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FileProcessingRecords]') AND type in (N'U'))
DROP TABLE [dbo].[FileProcessingRecords]
GO
/****** Object:  Table [dbo].[CustomerRecord]    Script Date: 23-10-2024 12:47:59 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerRecord]') AND type in (N'U'))
DROP TABLE [dbo].[CustomerRecord]
GO
USE [master]
GO
/****** Object:  Database [FileProcessingDB]    Script Date: 23-10-2024 12:47:59 PM ******/
DROP DATABASE [FileProcessingDB]
GO
/****** Object:  Database [FileProcessingDB]    Script Date: 23-10-2024 12:47:59 PM ******/
CREATE DATABASE [FileProcessingDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'FileProcessingDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\FileProcessingDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'FileProcessingDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\FileProcessingDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [FileProcessingDB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [FileProcessingDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [FileProcessingDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [FileProcessingDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [FileProcessingDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [FileProcessingDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [FileProcessingDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [FileProcessingDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [FileProcessingDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [FileProcessingDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [FileProcessingDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [FileProcessingDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [FileProcessingDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [FileProcessingDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [FileProcessingDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [FileProcessingDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [FileProcessingDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [FileProcessingDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [FileProcessingDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [FileProcessingDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [FileProcessingDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [FileProcessingDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [FileProcessingDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [FileProcessingDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [FileProcessingDB] SET RECOVERY FULL 
GO
ALTER DATABASE [FileProcessingDB] SET  MULTI_USER 
GO
ALTER DATABASE [FileProcessingDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [FileProcessingDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [FileProcessingDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [FileProcessingDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [FileProcessingDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [FileProcessingDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'FileProcessingDB', N'ON'
GO
ALTER DATABASE [FileProcessingDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [FileProcessingDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [FileProcessingDB]
GO
/****** Object:  Table [dbo].[CustomerRecord]    Script Date: 23-10-2024 12:48:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerRecord](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SlNo] [int] NULL,
	[CustomerId] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[Company] [varchar](50) NULL,
	[City] [varchar](50) NULL,
	[Country] [varchar](50) NULL,
	[Phone1] [varchar](50) NULL,
	[Phone2] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[Website] [varchar](50) NULL,
 CONSTRAINT [PK_ConsumerRecord] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FileProcessingRecords]    Script Date: 23-10-2024 12:48:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileProcessingRecords](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[Status] [int] NOT NULL,
	[ProcessStartTime] [datetime] NULL,
	[ProcessEndTime] [datetime] NULL,
 CONSTRAINT [PK__FileProc__3214EC07019F8A06] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Status]    Script Date: 23-10-2024 12:48:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Status](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StatusName] [varchar](50) NULL,
 CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 23-10-2024 12:48:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](50) NULL,
	[Password] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Status] ON 

INSERT [dbo].[Status] ([Id], [StatusName]) VALUES (1, N'Pending')
INSERT [dbo].[Status] ([Id], [StatusName]) VALUES (2, N'InProgress')
INSERT [dbo].[Status] ([Id], [StatusName]) VALUES (3, N'Completed')
INSERT [dbo].[Status] ([Id], [StatusName]) VALUES (4, N'Error')
SET IDENTITY_INSERT [dbo].[Status] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([Id], [Username], [Password], [Email]) VALUES (2, N'umesh', N'Warning@2', N'umesh@gmail.com')
INSERT [dbo].[Users] ([Id], [Username], [Password], [Email]) VALUES (3, N'Lekshmi', N'lekshmi@123', N'lekshmi@gmail.com')
INSERT [dbo].[Users] ([Id], [Username], [Password], [Email]) VALUES (4, N'Dhaksh', N'dhaksh@123', N'dhaksh@gmail.com')
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
ALTER TABLE [dbo].[FileProcessingRecords]  WITH CHECK ADD  CONSTRAINT [FK_FileProcessingRecords_Status] FOREIGN KEY([Status])
REFERENCES [dbo].[Status] ([Id])
GO
ALTER TABLE [dbo].[FileProcessingRecords] CHECK CONSTRAINT [FK_FileProcessingRecords_Status]
GO
USE [master]
GO
ALTER DATABASE [FileProcessingDB] SET  READ_WRITE 
GO
