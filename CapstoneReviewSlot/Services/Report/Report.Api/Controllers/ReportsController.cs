using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Report.Application.Interfaces;
using Report.Domain.DTOs;

namespace Report.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService) => _reportService = reportService;

    /// <summary>Get the review timeline for a campaign.</summary>
    [HttpGet("timeline/{campaignId:guid}")]
    [Authorize(Roles = "Admin,Manager,Lecturer")]
    [ProducesResponseType(typeof(ReviewTimelineDto), 200)]
    public async Task<IActionResult> GetTimeline(Guid campaignId, CancellationToken ct)
    {
        var timeline = await _reportService.GetReviewTimelineAsync(campaignId, ct);
        return Ok(timeline);
    }

    /// <summary>Get lecturer workload report for a campaign.</summary>
    [HttpGet("workload/{campaignId:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(WorkloadReportDto), 200)]
    public async Task<IActionResult> GetWorkload(Guid campaignId, CancellationToken ct)
    {
        var workload = await _reportService.GetWorkloadReportAsync(campaignId, ct);
        return Ok(workload);
    }

    /// <summary>Export review timeline as Excel file.</summary>
    [HttpGet("timeline/{campaignId:guid}/export")]
    [Authorize(Roles = "Admin,Manager")]
    [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    public async Task<IActionResult> ExportTimeline(Guid campaignId, CancellationToken ct)
    {
        var data = await _reportService.ExportTimelineAsync(campaignId, ct);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"review-timeline-{campaignId}.xlsx");
    }

    /// <summary>Export workload report as Excel file.</summary>
    [HttpGet("workload/{campaignId:guid}/export")]
    [Authorize(Roles = "Admin,Manager")]
    [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    public async Task<IActionResult> ExportWorkload(Guid campaignId, CancellationToken ct)
    {
        var data = await _reportService.ExportWorkloadAsync(campaignId, ct);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"workload-report-{campaignId}.xlsx");
    }
}
