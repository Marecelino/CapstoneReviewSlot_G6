-- =============================================
-- AvailabilityDB — dành cho Availability Service
-- Chứa: LecturerAvailability
-- LecturerId  → logic ref sang IdentityDB (không FK cứng)
-- ReviewSlotId → logic ref sang SessionDB  (không FK cứng)
-- =============================================
IF DB_ID(N'AvailabilityDB') IS NULL
    CREATE DATABASE AvailabilityDB;
GO
USE AvailabilityDB;
GO

IF OBJECT_ID('dbo.LecturerAvailability','U') IS NOT NULL DROP TABLE dbo.LecturerAvailability;
GO

CREATE TABLE dbo.LecturerAvailability
(
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    LecturerId   INT          NOT NULL,   -- ID logic từ IdentityDB.Lecturer
    ReviewSlotId UNIQUEIDENTIFIER NOT NULL, -- ID logic từ SessionDB.ReviewSlot
    Status       NVARCHAR(30) NOT NULL DEFAULT 'Available',
    -- Status: 'Available' | 'Cancelled' | 'Assigned'
    RegisteredAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),

    -- Mỗi giảng viên chỉ đăng ký tối đa 1 lần mỗi slot
    CONSTRAINT UQ_LecturerAvailability UNIQUE (LecturerId, ReviewSlotId),

    CONSTRAINT CHK_LecturerAvailability_Status
        CHECK (Status IN ('Available', 'Cancelled', 'Assigned'))
);
GO

CREATE INDEX IX_LecturerAvailability_LecturerId   ON dbo.LecturerAvailability(LecturerId);
CREATE INDEX IX_LecturerAvailability_ReviewSlotId ON dbo.LecturerAvailability(ReviewSlotId);
GO
