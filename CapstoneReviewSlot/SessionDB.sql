IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416175237_InitSession'
)
BEGIN
    CREATE TABLE [ReviewCampaign] (
        [CampaignId] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [StartTime] datetime2 NOT NULL,
        [EndTime] datetime2 NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        CONSTRAINT [PK_ReviewCampaign] PRIMARY KEY ([CampaignId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416175237_InitSession'
)
BEGIN
    CREATE TABLE [ReviewSlot] (
        [ReviewSlotId] uniqueidentifier NOT NULL,
        [CampaignId] uniqueidentifier NOT NULL,
        [ReviewDate] datetime2 NOT NULL,
        [SlotNumber] int NOT NULL,
        [StartTime] datetime2 NOT NULL,
        [EndTime] datetime2 NOT NULL,
        [Room] nvarchar(100) NOT NULL,
        [MaxCapacity] int NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        CONSTRAINT [PK_ReviewSlot] PRIMARY KEY ([ReviewSlotId]),
        CONSTRAINT [FK_ReviewSlot_ReviewCampaign_CampaignId] FOREIGN KEY ([CampaignId]) REFERENCES [ReviewCampaign] ([CampaignId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416175237_InitSession'
)
BEGIN
    CREATE INDEX [IX_ReviewSlot_CampaignId] ON [ReviewSlot] ([CampaignId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416175237_InitSession'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260416175237_InitSession', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416200045_InitialSessionMigration'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ReviewSlot]') AND [c].[name] = N'StartTime');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [ReviewSlot] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [ReviewSlot] ALTER COLUMN [StartTime] time NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416200045_InitialSessionMigration'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ReviewSlot]') AND [c].[name] = N'ReviewDate');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [ReviewSlot] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [ReviewSlot] ALTER COLUMN [ReviewDate] date NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416200045_InitialSessionMigration'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ReviewSlot]') AND [c].[name] = N'EndTime');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [ReviewSlot] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [ReviewSlot] ALTER COLUMN [EndTime] time NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416200045_InitialSessionMigration'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ReviewCampaign]') AND [c].[name] = N'Status');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [ReviewCampaign] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [ReviewCampaign] ALTER COLUMN [Status] int NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416200045_InitialSessionMigration'
)
BEGIN
    ALTER TABLE [ReviewCampaign] ADD [MaxGroupsPerLecturer] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416200045_InitialSessionMigration'
)
BEGIN
    ALTER TABLE [ReviewCampaign] ADD [RequiredReviewersPerGroup] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416200045_InitialSessionMigration'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260416200045_InitialSessionMigration', N'8.0.0');
END;
GO

COMMIT;
GO

