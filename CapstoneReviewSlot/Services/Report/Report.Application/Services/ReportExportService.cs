using ClosedXML.Excel;
using Report.Application.Interfaces;
using Report.Domain.DTOs;

namespace Report.Application.Services;

public class ReportExportService : IReportExporter
{
    public byte[] ExportTimelineToExcel(ReviewTimelineDto timeline)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Review Timeline");

        var headers = new[] { "Week", "Day", "Slot", "Group Code", "Project Code", "Project Name (EN)", "Date", "Day", "Room", "Reviewer 1", "Reviewer 2" };
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
            ws.Cell(1, i + 1).Style.Font.Bold = true;
            ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        int row = 2;
        foreach (var item in timeline.Rows)
        {
            ws.Cell(row, 1).Value = item.WeekCode;
            ws.Cell(row, 2).Value = item.DayCode;
            ws.Cell(row, 3).Value = item.SlotCode;
            ws.Cell(row, 4).Value = item.GroupCode;
            ws.Cell(row, 5).Value = item.ProjectCode;
            ws.Cell(row, 6).Value = item.ProjectNameEn;
            ws.Cell(row, 7).Value = item.Date.ToString("yyyy-MM-dd");
            ws.Cell(row, 8).Value = item.DayOfWeek;
            ws.Cell(row, 9).Value = item.Room;
            ws.Cell(row, 10).Value = item.Reviewer1Name;
            ws.Cell(row, 11).Value = item.Reviewer2Name;
            row++;
        }

        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] ExportWorkloadToExcel(WorkloadReportDto report)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Workload");

        var headers = new[] { "Lecturer Name", "Department", "Review Count", "Defense Count", "Total" };
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
            ws.Cell(1, i + 1).Style.Font.Bold = true;
            ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        int row = 2;
        foreach (var lw in report.LecturerWorkloads.OrderByDescending(l => l.Total))
        {
            ws.Cell(row, 1).Value = lw.LecturerName;
            ws.Cell(row, 2).Value = lw.Department;
            ws.Cell(row, 3).Value = lw.ReviewCount;
            ws.Cell(row, 4).Value = lw.DefenseCount;
            ws.Cell(row, 5).Value = lw.Total;
            row++;
        }

        row++;
        ws.Cell(row, 1).Value = "TOTAL";
        ws.Cell(row, 3).Value = report.TotalReviews;
        ws.Cell(row, 4).Value = report.TotalDefense;
        ws.Cell(row, 5).Value = report.TotalReviews + report.TotalDefense;
        ws.Cell(row, 1).Style.Font.Bold = true;

        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
