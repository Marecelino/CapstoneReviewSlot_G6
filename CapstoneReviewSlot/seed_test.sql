USE IdentityDB;
GO
INSERT INTO dbo.[User] (Email, FullName, PasswordHash, Role, Status)
VALUES ('lecturer1@capstone.edu.vn', N'Nguyen Van Lecturer', '$2a$11$E0sW/3O2f2R1sH1I38.x/.QkL49a8uN4yH0J.i1k05cE3Q7mJ72K.', 'Lecturer', 'Active');
DECLARE @UserId INT = SCOPE_IDENTITY();
INSERT INTO dbo.Lecturer (UserId, LecturerCode, Department)
VALUES (@UserId, 'L001', 'SE');
GO

USE SessionDB;
GO
INSERT INTO dbo.ReviewCampaign (Name, StartTime, EndTime, Status)
VALUES ('Fall 2026 Review', '2026-05-01 00:00:00', '2026-06-01 00:00:00', 'Open');
DECLARE @CampaignId INT = SCOPE_IDENTITY();
INSERT INTO dbo.ReviewSlot (CampaignId, ReviewDate, SlotNumber, StartTime, EndTime, MaxCapacity)
VALUES 
(@CampaignId, '2026-05-15', 1, '07:30:00', '09:00:00', 10),
(@CampaignId, '2026-05-15', 2, '09:30:00', '11:00:00', 10);
GO
