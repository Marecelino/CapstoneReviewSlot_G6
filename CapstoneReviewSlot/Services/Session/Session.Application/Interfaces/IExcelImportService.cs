using Session.Domain.DTOs;

namespace Session.Application.Interfaces;

/// <summary>
/// Accepts Stream (not IFormFile) so the Application layer stays framework-agnostic.
/// The controller is responsible for extracting the stream from IFormFile.
/// </summary>
public interface IExcelImportService
{
    /// <summary>
    /// Import groups from SE_CapstoneProject_FA25_Review.xlsx (Projects + Student sheets).
    /// </summary>
    Task<ImportResultDto> ImportGroupsAndStudentsAsync(Guid campaignId, Stream fileStream, string fileName, CancellationToken ct = default);

    /// <summary>
    /// Import lecturer availability from LecturerBookingReviewSlots_RegistrationForm.xlsx (Review sheet).
    /// </summary>
    Task<ImportResultDto> ImportAvailabilityAsync(Stream fileStream, string fileName, CancellationToken ct = default);
}
