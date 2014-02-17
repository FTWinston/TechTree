
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 02/17/2014 23:51:57
-- Generated from EDMX file: C:\Users\Winston\Documents\Visual Studio 2012\Projects\TechTree\TechTree\Models\GameModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [TechTree];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Games'
CREATE TABLE [dbo].[Games] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [StatusID] int  NOT NULL,
    [GameModeID] int  NOT NULL,
    [CurrentPlayerID] int  NULL,
    [CreatedOn] datetime  NOT NULL,
    [LastUpdated] datetime  NOT NULL
);
GO

-- Creating table 'Players'
CREATE TABLE [dbo].[Players] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Wins] int  NOT NULL,
    [Losses] int  NOT NULL
);
GO

-- Creating table 'GamePlayers'
CREATE TABLE [dbo].[GamePlayers] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [GameID] int  NOT NULL,
    [PlayerID] int  NULL,
    [Number] int  NOT NULL,
    [Active] bit  NOT NULL
);
GO

-- Creating table 'GameData'
CREATE TABLE [dbo].[GameData] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [GameID] int  NOT NULL,
    [TypeID] int  NOT NULL,
    [Data] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'GameModes'
CREATE TABLE [dbo].[GameModes] (
    [ID] int  NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [MinPlayers] int  NOT NULL,
    [MaxPlayers] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'Games'
ALTER TABLE [dbo].[Games]
ADD CONSTRAINT [PK_Games]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Players'
ALTER TABLE [dbo].[Players]
ADD CONSTRAINT [PK_Players]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'GamePlayers'
ALTER TABLE [dbo].[GamePlayers]
ADD CONSTRAINT [PK_GamePlayers]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'GameData'
ALTER TABLE [dbo].[GameData]
ADD CONSTRAINT [PK_GameData]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'GameModes'
ALTER TABLE [dbo].[GameModes]
ADD CONSTRAINT [PK_GameModes]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [GameID] in table 'GamePlayers'
ALTER TABLE [dbo].[GamePlayers]
ADD CONSTRAINT [FK_GamePlayers_Games]
    FOREIGN KEY ([GameID])
    REFERENCES [dbo].[Games]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GamePlayers_Games'
CREATE INDEX [IX_FK_GamePlayers_Games]
ON [dbo].[GamePlayers]
    ([GameID]);
GO

-- Creating foreign key on [CurrentPlayerID] in table 'Games'
ALTER TABLE [dbo].[Games]
ADD CONSTRAINT [FK_Games_Players]
    FOREIGN KEY ([CurrentPlayerID])
    REFERENCES [dbo].[Players]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Games_Players'
CREATE INDEX [IX_FK_Games_Players]
ON [dbo].[Games]
    ([CurrentPlayerID]);
GO

-- Creating foreign key on [PlayerID] in table 'GamePlayers'
ALTER TABLE [dbo].[GamePlayers]
ADD CONSTRAINT [FK_GamePlayers_Players]
    FOREIGN KEY ([PlayerID])
    REFERENCES [dbo].[Players]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GamePlayers_Players'
CREATE INDEX [IX_FK_GamePlayers_Players]
ON [dbo].[GamePlayers]
    ([PlayerID]);
GO

-- Creating foreign key on [GameID] in table 'GameData'
ALTER TABLE [dbo].[GameData]
ADD CONSTRAINT [FK_GameData_Games]
    FOREIGN KEY ([GameID])
    REFERENCES [dbo].[Games]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GameData_Games'
CREATE INDEX [IX_FK_GameData_Games]
ON [dbo].[GameData]
    ([GameID]);
GO

-- Creating foreign key on [GameModeID] in table 'Games'
ALTER TABLE [dbo].[Games]
ADD CONSTRAINT [FK_Games_GameModes]
    FOREIGN KEY ([GameModeID])
    REFERENCES [dbo].[GameModes]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Games_GameModes'
CREATE INDEX [IX_FK_Games_GameModes]
ON [dbo].[Games]
    ([GameModeID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------