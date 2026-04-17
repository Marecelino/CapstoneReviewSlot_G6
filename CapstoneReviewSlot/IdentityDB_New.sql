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

CREATE TABLE [User] (
    [UserId] uniqueidentifier NOT NULL,
    [Email] nvarchar(255) NOT NULL,
    [FullName] nvarchar(200) NOT NULL,
    [PasswordHash] nvarchar(500) NOT NULL,
    [Role] nvarchar(50) NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_User] PRIMARY KEY ([UserId])
);
GO

CREATE UNIQUE INDEX [IX_User_Email] ON [User] ([Email]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260416170721_InitIdentity', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [User] ADD [Status] nvarchar(30) NOT NULL DEFAULT N'';
GO

CREATE TABLE [Lecturer] (
    [LecturerId] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [LecturerCode] nvarchar(50) NOT NULL,
    [Department] nvarchar(100) NULL,
    CONSTRAINT [PK_Lecturer] PRIMARY KEY ([LecturerId]),
    CONSTRAINT [FK_Lecturer_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([UserId]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_Lecturer_LecturerCode] ON [Lecturer] ([LecturerCode]);
GO

CREATE UNIQUE INDEX [IX_Lecturer_UserId] ON [Lecturer] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260416193811_FixUserLecturerSchema', N'8.0.0');
GO

COMMIT;
GO

