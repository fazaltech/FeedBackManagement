
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 09/02/2019 11:17:15
-- Generated from EDMX file: C:\FeedBackManagement\FeedBackManagement\Models\Feedbackmodel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Feedback];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[feedbacks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[feedbacks];
GO
IF OBJECT_ID(N'[dbo].[fields]', 'U') IS NOT NULL
    DROP TABLE [dbo].[fields];
GO
IF OBJECT_ID(N'[dbo].[users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[users];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'feedbacks'
CREATE TABLE [dbo].[feedbacks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [type] varchar(50)  NULL,
    [summary] varchar(50)  NULL,
    [description] varchar(max)  NULL,
    [filename] varchar(50)  NULL,
    [filepath] varchar(max)  NULL,
    [employee_name] varchar(50)  NULL,
    [emp_field] varchar(50)  NULL,
    [date_time] datetime  NULL,
    [status] varchar(50)  NULL
);
GO

-- Creating table 'users'
CREATE TABLE [dbo].[users] (
    [Id] int  NOT NULL,
    [user_name] varchar(50)  NULL,
    [email_id] varchar(50)  NULL,
    [password] varchar(50)  NULL,
    [role] varchar(50)  NULL
);
GO

-- Creating table 'fields'
CREATE TABLE [dbo].[fields] (
    [Id] int  NOT NULL,
    [field_name] varchar(50)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'feedbacks'
ALTER TABLE [dbo].[feedbacks]
ADD CONSTRAINT [PK_feedbacks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'users'
ALTER TABLE [dbo].[users]
ADD CONSTRAINT [PK_users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'fields'
ALTER TABLE [dbo].[fields]
ADD CONSTRAINT [PK_fields]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------