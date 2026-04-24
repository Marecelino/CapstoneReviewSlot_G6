using Report.Domain.DTOs;

namespace Report.Application.Interfaces;

public interface IReportService
{
    Task<ReviewTimelineDto> GetReviewTimelineAsync(Guid campaignId, CancellationToken ct = default);
    Task<WorkloadReportDto> GetWorkloadReportAsync(Guid campaignId, CancellationToken ct = default);
    Task<byte[]> ExportTimelineAsync(Guid campaignId, CancellationToken ct = default);
    Task<byte[]> ExportWorkloadAsync(Guid campaignId, CancellationToken ct = default);
}
