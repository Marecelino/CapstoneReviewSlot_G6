using Report.Domain.DTOs;

namespace Report.Application.Interfaces;

public interface IReportExporter
{
    byte[] ExportTimelineToExcel(ReviewTimelineDto timeline);
    byte[] ExportWorkloadToExcel(WorkloadReportDto report);
}
