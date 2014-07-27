
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Wallet];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_AccountWalletTransaction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WalletTransactions] DROP CONSTRAINT [FK_AccountWalletTransaction];
GO
IF OBJECT_ID(N'[dbo].[FK_AccountStateAccount]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Accounts] DROP CONSTRAINT [FK_AccountStateAccount];
GO
IF OBJECT_ID(N'[dbo].[FK_TransactionTypeWalletTransaction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WalletTransactions] DROP CONSTRAINT [FK_TransactionTypeWalletTransaction];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Accounts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Accounts];
GO
IF OBJECT_ID(N'[dbo].[WalletTransactions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WalletTransactions];
GO
IF OBJECT_ID(N'[dbo].[AccountStates]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AccountStates];
GO
IF OBJECT_ID(N'[dbo].[TransactionTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TransactionTypes];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Accounts'
CREATE TABLE [dbo].[Accounts] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [UserID] nvarchar(max)  NOT NULL,
    [Balance] decimal(18,2)  NOT NULL,
    [AccountStateId] int  NOT NULL,
    [DateCreated] datetime  NULL,
    [DateClosed] datetime  NULL
);
GO

-- Creating table 'WalletTransactions'
CREATE TABLE [dbo].[WalletTransactions] (
    [TransactionID] bigint IDENTITY(1,1) NOT NULL,
    [AccountID] bigint  NOT NULL,
    [TransactionTypeId] int  NOT NULL,
    [DateCreated] datetime  NULL,
    [Amount] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'AccountStates'
CREATE TABLE [dbo].[AccountStates] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [State] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'TransactionTypes'
CREATE TABLE [dbo].[TransactionTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Type] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'Accounts'
ALTER TABLE [dbo].[Accounts]
ADD CONSTRAINT [PK_Accounts]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [TransactionID] in table 'WalletTransactions'
ALTER TABLE [dbo].[WalletTransactions]
ADD CONSTRAINT [PK_WalletTransactions]
    PRIMARY KEY CLUSTERED ([TransactionID] ASC);
GO

-- Creating primary key on [Id] in table 'AccountStates'
ALTER TABLE [dbo].[AccountStates]
ADD CONSTRAINT [PK_AccountStates]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TransactionTypes'
ALTER TABLE [dbo].[TransactionTypes]
ADD CONSTRAINT [PK_TransactionTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [AccountID] in table 'WalletTransactions'
ALTER TABLE [dbo].[WalletTransactions]
ADD CONSTRAINT [FK_AccountWalletTransaction]
    FOREIGN KEY ([AccountID])
    REFERENCES [dbo].[Accounts]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AccountWalletTransaction'
CREATE INDEX [IX_FK_AccountWalletTransaction]
ON [dbo].[WalletTransactions]
    ([AccountID]);
GO

-- Creating foreign key on [AccountStateId] in table 'Accounts'
ALTER TABLE [dbo].[Accounts]
ADD CONSTRAINT [FK_AccountStateAccount]
    FOREIGN KEY ([AccountStateId])
    REFERENCES [dbo].[AccountStates]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AccountStateAccount'
CREATE INDEX [IX_FK_AccountStateAccount]
ON [dbo].[Accounts]
    ([AccountStateId]);
GO

-- Creating foreign key on [TransactionTypeId] in table 'WalletTransactions'
ALTER TABLE [dbo].[WalletTransactions]
ADD CONSTRAINT [FK_TransactionTypeWalletTransaction]
    FOREIGN KEY ([TransactionTypeId])
    REFERENCES [dbo].[TransactionTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TransactionTypeWalletTransaction'
CREATE INDEX [IX_FK_TransactionTypeWalletTransaction]
ON [dbo].[WalletTransactions]
    ([TransactionTypeId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------