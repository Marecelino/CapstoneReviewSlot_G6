-- Seed IdentityDB
USE IdentityDB;
GO
-- Insert User
INSERT INTO dbo.[User] (UserId, Email, FullName, PasswordHash, Role, Status, CreatedAtUtc, IsDeleted)
VALUES ('d5c4b3a2-e1f0-4d9c-8b7a-6a5b4c3d2e1f', 'lecturer1@capstone.edu.vn', N'Nguyen Van Lecturer', '$2a$11$E0sW/3O2f2R1sH1I38.x/.QkL49a8uN4yH0J.i1k05cE3Q7mJ72K.', 'Lecturer', 'Active', GETUTCDATE(), 0);

-- Insert Lecturer (LecturerId is now Guid!)
INSERT INTO dbo.Lecturer (LecturerId, UserId, LecturerCode, Department)
VALUES ('e8f7d6c5-b4a3-4217-90c8-b7a6d5c4e3f2', 'd5c4b3a2-e1f0-4d9c-8b7a-6a5b4c3d2e1f', 'L001', 'SE');
GO

-- Seed SessionDB
USE SessionDB;
GO
-- Insert Campaign
INSERT INTO dbo.ReviewCampaign (CampaignId, Name, StartTime, EndTime, Status, MaxGroupsPerLecturer, RequiredReviewersPerGroup, CreatedAtUtc, IsDeleted)
VALUES ('a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d', 'Fall 2026 Review', '2026-05-01 00:00:00', '2026-06-01 00:00:00', 1, 10, 3, GETUTCDATE(), 0);

-- Insert Slot
INSERT INTO dbo.ReviewSlot (ReviewSlotId, CampaignId, ReviewDate, SlotNumber, StartTime, EndTime, Room, MaxCapacity, CreatedAtUtc, IsDeleted)
VALUES ('f9e8d7c6-b5a4-4938-8726-1029384756ad', 'a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d', '2026-05-15', 1, '07:30:00', '09:00:00', 'Room 101', 10, GETUTCDATE(), 0);
GO
