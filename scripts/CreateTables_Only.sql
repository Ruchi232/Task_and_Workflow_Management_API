-- ============================================
-- Create tables in existing Ruchi_TaskWorkflowDb
-- 1. In SSMS, connect to XIT037\MSSQLSERVER2022 as test.practical
-- 2. Select database Ruchi_TaskWorkflowDb (or run USE below)
-- 3. Execute this script (F5)
-- ============================================

USE [Ruchi_TaskWorkflowDb];
GO

-- Create Projects table (if not exists)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'Projects')
BEGIN
    CREATE TABLE [dbo].[Projects] (
        [Id]          INT            IDENTITY (1, 1) NOT NULL,
        [Name]        NVARCHAR (200) NOT NULL,
        [Description] NVARCHAR (1000) NULL,
        [CreatedAt]   DATETIME2 (7)  NOT NULL,
        CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Table Projects created.';
END
ELSE
    PRINT 'Table Projects already exists.';
GO

-- Create TaskItems table (if not exists)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'TaskItems')
BEGIN
    CREATE TABLE [dbo].[TaskItems] (
        [Id]          INT            IDENTITY (1, 1) NOT NULL,
        [ProjectId]   INT            NOT NULL,
        [Title]       NVARCHAR (300) NOT NULL,
        [Description] NVARCHAR (2000) NULL,
        [Status]      INT            NOT NULL,
        [Priority]    INT            NOT NULL,
        [DueDate]     DATETIME2 (7)  NULL,
        CONSTRAINT [PK_TaskItems] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_TaskItems_Projects_ProjectId] FOREIGN KEY ([ProjectId])
            REFERENCES [dbo].[Projects] ([Id]) ON DELETE NO ACTION
    );

    CREATE NONCLUSTERED INDEX [IX_TaskItems_ProjectId]
        ON [dbo].[TaskItems] ([ProjectId] ASC);
    PRINT 'Table TaskItems created.';
END
ELSE
    PRINT 'Table TaskItems already exists.';
GO

-- EF Core migrations history (so EF thinks InitialCreate was already applied)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'__EFMigrationsHistory')
BEGIN
    CREATE TABLE [dbo].[__EFMigrationsHistory] (
        [MigrationId]    NVARCHAR (150) NOT NULL,
        [ProductVersion] NVARCHAR (32)  NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED ([MigrationId] ASC)
    );
    PRINT 'Table __EFMigrationsHistory created.';
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = N'20250131120000_InitialCreate')
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250131120000_InitialCreate', N'8.0.11');
    PRINT 'Migration history row inserted.';
END
GO

PRINT 'Done. Tables in Ruchi_TaskWorkflowDb are ready.';
