
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 11/30/2012 17:34:56
-- Generated from EDMX file: C:\Users\Barry\documents\visual studio 2012\Projects\DelayRepay_Service\DelayRepay_BL\DR_Model.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [DelayRepay];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_StationUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_StationUser];
GO
IF OBJECT_ID(N'[dbo].[FK_UserStation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_UserStation];
GO
IF OBJECT_ID(N'[dbo].[FK_LogTypeLog]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Logs] DROP CONSTRAINT [FK_LogTypeLog];
GO
IF OBJECT_ID(N'[dbo].[FK_JourneyDestination]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Destinations] DROP CONSTRAINT [FK_JourneyDestination];
GO
IF OBJECT_ID(N'[dbo].[FK_StationDestination]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Destinations] DROP CONSTRAINT [FK_StationDestination];
GO
IF OBJECT_ID(N'[dbo].[FK_DestinationFromStation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FromStations] DROP CONSTRAINT [FK_DestinationFromStation];
GO
IF OBJECT_ID(N'[dbo].[FK_StationFromStation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FromStations] DROP CONSTRAINT [FK_StationFromStation];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Stations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Stations];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[Logs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Logs];
GO
IF OBJECT_ID(N'[dbo].[LogTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LogTypes];
GO
IF OBJECT_ID(N'[dbo].[Journeys]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Journeys];
GO
IF OBJECT_ID(N'[dbo].[Destinations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Destinations];
GO
IF OBJECT_ID(N'[dbo].[FromStations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FromStations];
GO
IF OBJECT_ID(N'[dbo].[EmailBatches]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EmailBatches];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Stations'
CREATE TABLE [dbo].[Stations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [StationCode] nvarchar(max)  NOT NULL,
    [StationName] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FormsAuthUserID] int  NOT NULL,
    [HomeStationId] int  NOT NULL,
    [DestinationStationId] int  NOT NULL
);
GO

-- Creating table 'Logs'
CREATE TABLE [dbo].[Logs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [LogTypeId] int  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [Message] nvarchar(max)  NOT NULL,
    [Header] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'LogTypes'
CREATE TABLE [dbo].[LogTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [LogTypeName] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Journeys'
CREATE TABLE [dbo].[Journeys] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [JCode] nvarchar(max)  NOT NULL,
    [TrainOperator] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Destinations'
CREATE TABLE [dbo].[Destinations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [JourneyId] int  NOT NULL,
    [StationId] int  NOT NULL,
    [ScheduledArrival] datetime  NOT NULL,
    [ActualArrival] datetime  NOT NULL,
    [EmailBatchId] int  NULL
);
GO

-- Creating table 'FromStations'
CREATE TABLE [dbo].[FromStations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DestinationId] int  NOT NULL,
    [StationId] int  NOT NULL
);
GO

-- Creating table 'EmailBatches'
CREATE TABLE [dbo].[EmailBatches] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Created] datetime  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Stations'
ALTER TABLE [dbo].[Stations]
ADD CONSTRAINT [PK_Stations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Logs'
ALTER TABLE [dbo].[Logs]
ADD CONSTRAINT [PK_Logs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'LogTypes'
ALTER TABLE [dbo].[LogTypes]
ADD CONSTRAINT [PK_LogTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Journeys'
ALTER TABLE [dbo].[Journeys]
ADD CONSTRAINT [PK_Journeys]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Destinations'
ALTER TABLE [dbo].[Destinations]
ADD CONSTRAINT [PK_Destinations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FromStations'
ALTER TABLE [dbo].[FromStations]
ADD CONSTRAINT [PK_FromStations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EmailBatches'
ALTER TABLE [dbo].[EmailBatches]
ADD CONSTRAINT [PK_EmailBatches]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [HomeStationId] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_StationUser]
    FOREIGN KEY ([HomeStationId])
    REFERENCES [dbo].[Stations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StationUser'
CREATE INDEX [IX_FK_StationUser]
ON [dbo].[Users]
    ([HomeStationId]);
GO

-- Creating foreign key on [DestinationStationId] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_UserStation]
    FOREIGN KEY ([DestinationStationId])
    REFERENCES [dbo].[Stations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserStation'
CREATE INDEX [IX_FK_UserStation]
ON [dbo].[Users]
    ([DestinationStationId]);
GO

-- Creating foreign key on [LogTypeId] in table 'Logs'
ALTER TABLE [dbo].[Logs]
ADD CONSTRAINT [FK_LogTypeLog]
    FOREIGN KEY ([LogTypeId])
    REFERENCES [dbo].[LogTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LogTypeLog'
CREATE INDEX [IX_FK_LogTypeLog]
ON [dbo].[Logs]
    ([LogTypeId]);
GO

-- Creating foreign key on [JourneyId] in table 'Destinations'
ALTER TABLE [dbo].[Destinations]
ADD CONSTRAINT [FK_JourneyDestination]
    FOREIGN KEY ([JourneyId])
    REFERENCES [dbo].[Journeys]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_JourneyDestination'
CREATE INDEX [IX_FK_JourneyDestination]
ON [dbo].[Destinations]
    ([JourneyId]);
GO

-- Creating foreign key on [StationId] in table 'Destinations'
ALTER TABLE [dbo].[Destinations]
ADD CONSTRAINT [FK_StationDestination]
    FOREIGN KEY ([StationId])
    REFERENCES [dbo].[Stations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StationDestination'
CREATE INDEX [IX_FK_StationDestination]
ON [dbo].[Destinations]
    ([StationId]);
GO

-- Creating foreign key on [DestinationId] in table 'FromStations'
ALTER TABLE [dbo].[FromStations]
ADD CONSTRAINT [FK_DestinationFromStation]
    FOREIGN KEY ([DestinationId])
    REFERENCES [dbo].[Destinations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DestinationFromStation'
CREATE INDEX [IX_FK_DestinationFromStation]
ON [dbo].[FromStations]
    ([DestinationId]);
GO

-- Creating foreign key on [StationId] in table 'FromStations'
ALTER TABLE [dbo].[FromStations]
ADD CONSTRAINT [FK_StationFromStation]
    FOREIGN KEY ([StationId])
    REFERENCES [dbo].[Stations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StationFromStation'
CREATE INDEX [IX_FK_StationFromStation]
ON [dbo].[FromStations]
    ([StationId]);
GO

-- Creating foreign key on [EmailBatchId] in table 'Destinations'
ALTER TABLE [dbo].[Destinations]
ADD CONSTRAINT [FK_EmailBatchDestination]
    FOREIGN KEY ([EmailBatchId])
    REFERENCES [dbo].[EmailBatches]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_EmailBatchDestination'
CREATE INDEX [IX_FK_EmailBatchDestination]
ON [dbo].[Destinations]
    ([EmailBatchId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------