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

CREATE TABLE [LecturerAvailability] (
    [AvailabilityId] uniqueidentifier NOT NULL,
    [LecturerId] uniqueidentifier NOT NULL,
    [ReviewSlotId] uniqueidentifier NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_LecturerAvailability] PRIMARY KEY ([AvailabilityId])
);
GO

CREATE UNIQUE INDEX [IX_LecturerAvailability_LecturerId_ReviewSlotId] ON [LecturerAvailability] ([LecturerId], [ReviewSlotId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260416182226_InitAvailability', N'8.0.0');
GO

COMMIT;
GO

