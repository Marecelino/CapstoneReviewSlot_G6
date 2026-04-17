-- =============================================
-- 1) CREATE DATABASE
-- =============================================
IF DB_ID(N'CapstoneReviewDB') IS NULL
BEGIN
    CREATE DATABASE CapstoneReviewDB;
END
GO

USE CapstoneReviewDB;
GO

-- =============================================
-- 2) DROP TABLES IF EXIST
-- =============================================
IF OBJECT_ID('dbo.ReviewAssignmentReviewer', 'U') IS NOT NULL DROP TABLE dbo.ReviewAssignmentReviewer;
IF OBJECT_ID('dbo.LecturerAvailability', 'U') IS NOT NULL DROP TABLE dbo.LecturerAvailability;
IF OBJECT_ID('dbo.ReviewAssignment', 'U') IS NOT NULL DROP TABLE dbo.ReviewAssignment;
IF OBJECT_ID('dbo.CapstoneGroup', 'U') IS NOT NULL DROP TABLE dbo.CapstoneGroup;
IF OBJECT_ID('dbo.ReviewSlot', 'U') IS NOT NULL DROP TABLE dbo.ReviewSlot;
IF OBJECT_ID('dbo.ReviewCampaign', 'U') IS NOT NULL DROP TABLE dbo.ReviewCampaign;
IF OBJECT_ID('dbo.Lecturer', 'U') IS NOT NULL DROP TABLE dbo.Lecturer;
IF OBJECT_ID('dbo.[User]', 'U') IS NOT NULL DROP TABLE dbo.[User];
GO

-- =============================================
-- 3) TABLE: User
-- =============================================
CREATE TABLE dbo.[User]
(
    UserId      INT IDENTITY(1,1) PRIMARY KEY,
    Email       NVARCHAR(255) NOT NULL,
    FullName    NVARCHAR(150) NOT NULL,
    Role        NVARCHAR(50) NOT NULL,

    CONSTRAINT UQ_User_Email UNIQUE (Email)
);
GO

-- =============================================
-- 4) TABLE: Lecturer
--    1 - 1 với User
-- =============================================
CREATE TABLE dbo.Lecturer
(
    LecturerId      INT IDENTITY(1,1) PRIMARY KEY,
    UserId          INT NOT NULL,
    LecturerCode    NVARCHAR(50) NOT NULL,
    Department      NVARCHAR(100) NULL,

    CONSTRAINT UQ_Lecturer_UserId UNIQUE (UserId),
    CONSTRAINT UQ_Lecturer_LecturerCode UNIQUE (LecturerCode),

    CONSTRAINT FK_Lecturer_User
        FOREIGN KEY (UserId) REFERENCES dbo.[User](UserId)
);
GO

-- =============================================
-- 5) TABLE: ReviewCampaign
-- =============================================
CREATE TABLE dbo.ReviewCampaign
(
    CampaignId                  INT IDENTITY(1,1) PRIMARY KEY,
    Name                        NVARCHAR(200) NOT NULL,
    StartTime                   DATETIME2(0) NOT NULL,
    EndTime                     DATETIME2(0) NOT NULL,
    MaxGroupsPerLecturer        INT NOT NULL,
    RequiredReviewersPerGroup   INT NOT NULL,
    Status                      NVARCHAR(30) NOT NULL,

    CONSTRAINT CHK_ReviewCampaign_Time CHECK (EndTime >= StartTime),
    CONSTRAINT CHK_ReviewCampaign_MaxGroups CHECK (MaxGroupsPerLecturer >= 0),
    CONSTRAINT CHK_ReviewCampaign_RequiredReviewers CHECK (RequiredReviewersPerGroup > 0)
);
GO

-- =============================================
-- 6) TABLE: ReviewSlot
--    1 Campaign có nhiều Slot
-- =============================================
CREATE TABLE dbo.ReviewSlot
(
    ReviewSlotId    INT IDENTITY(1,1) PRIMARY KEY,
    CampaignId      INT NOT NULL,
    ReviewDate      DATE NOT NULL,
    SlotNumber      INT NOT NULL,
    StartTime       TIME(0) NOT NULL,
    EndTime         TIME(0) NOT NULL,
    Room            NVARCHAR(50) NULL,
    MaxCapacity     INT NOT NULL,

    CONSTRAINT FK_ReviewSlot_ReviewCampaign
        FOREIGN KEY (CampaignId) REFERENCES dbo.ReviewCampaign(CampaignId),

    CONSTRAINT CHK_ReviewSlot_Time CHECK (EndTime > StartTime),
    CONSTRAINT CHK_ReviewSlot_MaxCapacity CHECK (MaxCapacity > 0),

    CONSTRAINT UQ_ReviewSlot_Campaign_Date_Slot UNIQUE (CampaignId, ReviewDate, SlotNumber)
);
GO

-- =============================================
-- 7) TABLE: CapstoneGroup
--    1 Lecturer mentor nhiều group
--    1 Campaign có nhiều group
-- =============================================
CREATE TABLE dbo.CapstoneGroup
(
    CapstoneGroupId     INT IDENTITY(1,1) PRIMARY KEY,
    MentorLecturerId    INT NOT NULL,
    CampaignId          INT NOT NULL,
    GroupCode           NVARCHAR(50) NOT NULL,
    SubjectCode         NVARCHAR(50) NOT NULL,
    ProjectName         NVARCHAR(255) NOT NULL,

    CONSTRAINT FK_CapstoneGroup_Lecturer
        FOREIGN KEY (MentorLecturerId) REFERENCES dbo.Lecturer(LecturerId),

    CONSTRAINT FK_CapstoneGroup_ReviewCampaign
        FOREIGN KEY (CampaignId) REFERENCES dbo.ReviewCampaign(CampaignId),

    CONSTRAINT UQ_CapstoneGroup_Campaign_GroupCode UNIQUE (CampaignId, GroupCode)
);
GO

-- =============================================
-- 8) TABLE: ReviewAssignment
--    Group được gán vào 1 Slot
-- =============================================
CREATE TABLE dbo.ReviewAssignment
(
    ReviewAssignmentId  INT IDENTITY(1,1) PRIMARY KEY,
    CapstoneGroupId     INT NOT NULL,
    ReviewSlotId        INT NOT NULL,
    Status              NVARCHAR(30) NOT NULL,
    ReviewOrder         INT NOT NULL,
    AssignedBy          INT NULL,
    AssignedAt          DATETIME2(0) NULL,

    CONSTRAINT FK_ReviewAssignment_CapstoneGroup
        FOREIGN KEY (CapstoneGroupId) REFERENCES dbo.CapstoneGroup(CapstoneGroupId),

    CONSTRAINT FK_ReviewAssignment_ReviewSlot
        FOREIGN KEY (ReviewSlotId) REFERENCES dbo.ReviewSlot(ReviewSlotId),

    CONSTRAINT UQ_ReviewAssignment_CapstoneGroup UNIQUE (CapstoneGroupId),
    CONSTRAINT UQ_ReviewAssignment_ReviewSlot_Order UNIQUE (ReviewSlotId, ReviewOrder),
    CONSTRAINT CHK_ReviewAssignment_ReviewOrder CHECK (ReviewOrder > 0)
);
GO

-- =============================================
-- 9) TABLE: LecturerAvailability
--    Lecturer đăng ký slot rảnh
-- =============================================
CREATE TABLE dbo.LecturerAvailability
(
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    LecturerId      INT NOT NULL,
    ReviewSlotId    INT NOT NULL,
    Status          NVARCHAR(30) NOT NULL,
    RegisteredAt    DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_LecturerAvailability_Lecturer
        FOREIGN KEY (LecturerId) REFERENCES dbo.Lecturer(LecturerId),

    CONSTRAINT FK_LecturerAvailability_ReviewSlot
        FOREIGN KEY (ReviewSlotId) REFERENCES dbo.ReviewSlot(ReviewSlotId),

    CONSTRAINT UQ_LecturerAvailability UNIQUE (LecturerId, ReviewSlotId)
);
GO

-- =============================================
-- 10) TABLE: ReviewAssignmentReviewer
--     Nhiều reviewer cho 1 assignment
-- =============================================
CREATE TABLE dbo.ReviewAssignmentReviewer
(
    ReviewerId          INT IDENTITY(1,1) PRIMARY KEY,
    LecturerId          INT NOT NULL,
    ReviewAssignmentId  INT NOT NULL,
    Role                NVARCHAR(50) NOT NULL,

    CONSTRAINT FK_ReviewAssignmentReviewer_Lecturer
        FOREIGN KEY (LecturerId) REFERENCES dbo.Lecturer(LecturerId),

    CONSTRAINT FK_ReviewAssignmentReviewer_ReviewAssignment
        FOREIGN KEY (ReviewAssignmentId) REFERENCES dbo.ReviewAssignment(ReviewAssignmentId),

    CONSTRAINT UQ_ReviewAssignmentReviewer UNIQUE (LecturerId, ReviewAssignmentId)
);
GO

-- =============================================
-- 11) INDEXES
-- =============================================
CREATE INDEX IX_Lecturer_UserId ON dbo.Lecturer(UserId);

CREATE INDEX IX_CapstoneGroup_MentorLecturerId ON dbo.CapstoneGroup(MentorLecturerId);
CREATE INDEX IX_CapstoneGroup_CampaignId ON dbo.CapstoneGroup(CampaignId);

CREATE INDEX IX_ReviewSlot_CampaignId ON dbo.ReviewSlot(CampaignId);

CREATE INDEX IX_ReviewAssignment_ReviewSlotId ON dbo.ReviewAssignment(ReviewSlotId);
CREATE INDEX IX_ReviewAssignment_CapstoneGroupId ON dbo.ReviewAssignment(CapstoneGroupId);

CREATE INDEX IX_LecturerAvailability_LecturerId ON dbo.LecturerAvailability(LecturerId);
CREATE INDEX IX_LecturerAvailability_ReviewSlotId ON dbo.LecturerAvailability(ReviewSlotId);

CREATE INDEX IX_ReviewAssignmentReviewer_LecturerId ON dbo.ReviewAssignmentReviewer(LecturerId);
CREATE INDEX IX_ReviewAssignmentReviewer_ReviewAssignmentId ON dbo.ReviewAssignmentReviewer(ReviewAssignmentId);
GO