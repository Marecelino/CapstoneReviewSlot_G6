using Assignment.Domain.Dtos;
using Assignment.Domain.Interfaces.Services;
using Assignment.Domain.Interfaces.Repositories;
using Assignment.Domain.Ultils;
using Session.Application.Ultils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Assignment.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ManagerAssignmentController : ControllerBase
{
    private readonly IReviewAssignmentService _assignmentService;
    private readonly IReviewAssignmentReviewerService _reviewerService;
    private readonly IHttpClientFactory _httpClientFactory;

    public ManagerAssignmentController(
        IReviewAssignmentService assignmentService,
        IReviewAssignmentReviewerService reviewerService,
        IHttpClientFactory httpClientFactory)
    {
        _assignmentService = assignmentService;
        _reviewerService = reviewerService;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("manual-assign")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> ManualAssign([FromBody] ManualAssignRequest request)
    {
        try
        {
            // 1. Create review assignment for the group-slot pair
            var assignmentRequest = new ReviewAssignmentRequest
            {
                CapstoneGroupId = request.CapstoneGroupId,
                ReviewSlotId = request.ReviewSlotId,
                AssignedBy = GetCurrentUserId()
            };

            var assignment = await _assignmentService.AddAsync(assignmentRequest);

            // 2. Add exactly 2 reviewers
            var reviewers = new List<ReviewAssignmentReviewerDto>
            {
                new ReviewAssignmentReviewerDto
                {
                    LecturerId = request.PrimaryReviewerId,
                    ReviewAssignmentId = assignment.Id,
                    Role = "PrimaryReviewer"
                },
                new ReviewAssignmentReviewerDto
                {
                    LecturerId = request.SecondaryReviewerId,
                    ReviewAssignmentId = assignment.Id,
                    Role = "SecondaryReviewer"
                }
            };

            var createdReviewers = await _reviewerService.AddAsync(reviewers);

            return Ok(ApiResult<object>.Success(new
            {
                Assignment = assignment,
                Reviewers = createdReviewers
            }, "201", "Phân công reviewer thành công."));
        }
        catch (Exception ex)
        {
            var statusCode = ExceptionUtils.ExtractStatusCode(ex);
            var errorResponse = ExceptionUtils.CreateErrorResponse<object>(ex);
            return StatusCode(statusCode, errorResponse);
        }
    }

    [HttpGet("campaign/{campaignId}/summary")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetWorkloadSummary(Guid campaignId)
    {
        try
        {
            var allAssignments = await _assignmentService.GetAllAsync();
            var allReviewers = await _reviewerService.GetAllAsync();

            // Get all slots for this campaign
            var slots = await GetCampaignSlotsAsync(campaignId);
            var slotIds = slots.Select(s => s.Id).ToHashSet();

            // Filter assignments and reviewers to this campaign
            var campaignAssignmentIds = allAssignments
                .Where(a => slotIds.Contains(a.ReviewSlotId))
                .Select(a => a.Id)
                .ToHashSet();

            var campaignReviewers = allReviewers
                .Where(r => campaignAssignmentIds.Contains(r.ReviewAssignmentId))
                .ToList();

            // Get lecturer info from Identity service
            var lecturerWorkloads = campaignReviewers
                .GroupBy(r => r.LecturerId)
                .Select(g => new LecturerWorkloadDto
                {
                    LecturerId = g.Key,
                    LecturerName = g.Key.ToString(), // fallback
                    TotalReviews = g.Count(),
                    PrimaryCount = g.Count(r => NormalizeRole(r.Role) == "primaryreviewer"),
                    SecondaryCount = g.Count(r => NormalizeRole(r.Role) == "secondaryreviewer")
                })
                .OrderByDescending(l => l.TotalReviews)
                .ToList();

            // Enrich with lecturer names
            foreach (var workload in lecturerWorkloads)
            {
                var name = await GetLecturerNameAsync(workload.LecturerId);
                if (!string.IsNullOrEmpty(name))
                    workload.LecturerName = name;
            }

            return Ok(ApiResult<object>.Success(lecturerWorkloads, "200", "Lấy workload summary thành công."));
        }
        catch (Exception ex)
        {
            var statusCode = ExceptionUtils.ExtractStatusCode(ex);
            var errorResponse = ExceptionUtils.CreateErrorResponse<object>(ex);
            return StatusCode(statusCode, errorResponse);
        }
    }

    [HttpGet("conflicts")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DetectConflicts()
    {
        try
        {
            var allAssignments = await _assignmentService.GetAllAsync();
            var allReviewers = await _reviewerService.GetAllAsync();
            var conflicts = new List<ConflictDto>();

            var slotGroups = allAssignments
                .GroupBy(a => a.ReviewSlotId)
                .Where(g => g.Count() > 1);

            foreach (var slotGroup in slotGroups)
            {
                var slotAssignmentIds = slotGroup.Select(a => a.Id).ToHashSet();
                var reviewersInSlot = allReviewers
                    .Where(r => slotAssignmentIds.Contains(r.ReviewAssignmentId))
                    .ToList();

                var lecturerSlotCounts = reviewersInSlot
                    .GroupBy(r => r.LecturerId)
                    .Where(g => g.Count() > 1);

                foreach (var lecturer in lecturerSlotCounts)
                {
                    conflicts.Add(new ConflictDto
                    {
                        LecturerId = lecturer.Key,
                        SlotId = slotGroup.Key,
                        ConflictType = "LECTURER_IN_SAME_SLOT",
                        Message = $"Giảng viên bị assign vào cùng một slot nhiều lần (count={lecturer.Count()})."
                    });
                }

                var roleCounts = reviewersInSlot
                    .GroupBy(r => NormalizeRole(r.Role))
                    .Where(g => g.Count() > 2);

                foreach (var role in roleCounts)
                {
                    conflicts.Add(new ConflictDto
                    {
                        SlotId = slotGroup.Key,
                        ConflictType = "TOO_MANY_SAME_ROLE",
                        Message = $"Slot có quá nhiều reviewer cùng role '{role.Key}' (count={role.Count()})."
                    });
                }
            }

            return Ok(ApiResult<object>.Success(conflicts, "200", $"Phát hiện {conflicts.Count} conflicts."));
        }
        catch (Exception ex)
        {
            var statusCode = ExceptionUtils.ExtractStatusCode(ex);
            var errorResponse = ExceptionUtils.CreateErrorResponse<object>(ex);
            return StatusCode(statusCode, errorResponse);
        }
    }

    [HttpGet("campaign/{campaignId}/unassigned")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetUnassignedGroups(Guid campaignId)
    {
        try
        {
            var slots = await GetCampaignSlotsAsync(campaignId);
            var slotIds = slots.Select(s => s.Id).ToHashSet();

            var allAssignments = await _assignmentService.GetAllAsync();
            var assignedGroupIds = allAssignments
                .Where(a => slotIds.Contains(a.ReviewSlotId))
                .Select(a => a.CapstoneGroupId)
                .ToHashSet();

            var allGroups = await GetCampaignGroupsAsync(campaignId);
            var unassigned = allGroups
                .Where(g => !assignedGroupIds.Contains(g.Id))
                .Select(g => new { g.Id, g.GroupCode, g.ProjectName })
                .ToList();

            return Ok(ApiResult<object>.Success(unassigned, "200", $"Có {unassigned.Count} nhóm chưa được phân công."));
        }
        catch (Exception ex)
        {
            var statusCode = ExceptionUtils.ExtractStatusCode(ex);
            var errorResponse = ExceptionUtils.CreateErrorResponse<object>(ex);
            return StatusCode(statusCode, errorResponse);
        }
    }

    private async Task<List<SlotInfo>> GetCampaignSlotsAsync(Guid campaignId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("SessionService");
            var response = await client.GetAsync($"/api/campaigns/{campaignId}/slots");
            if (!response.IsSuccessStatusCode) return new List<SlotInfo>();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var arr = doc.RootElement.GetProperty("value").GetProperty("data");

            var slots = new List<SlotInfo>();
            foreach (var item in arr.EnumerateArray())
            {
                slots.Add(new SlotInfo
                {
                    Id = Guid.Parse(item.GetProperty("id").GetString()!),
                    CampaignId = campaignId
                });
            }
            return slots;
        }
        catch
        {
            return new List<SlotInfo>();
        }
    }

    private async Task<List<GroupInfo>> GetCampaignGroupsAsync(Guid campaignId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("SessionService");
            var response = await client.GetAsync($"/api/groups/by-campaign/{campaignId}");
            if (!response.IsSuccessStatusCode) return new List<GroupInfo>();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var arr = doc.RootElement.GetProperty("value").GetProperty("data");

            var groups = new List<GroupInfo>();
            foreach (var item in arr.EnumerateArray())
            {
                groups.Add(new GroupInfo
                {
                    Id = Guid.Parse(item.GetProperty("id").GetString()!),
                    GroupCode = item.GetProperty("groupCode").GetString() ?? "",
                    ProjectName = item.TryGetProperty("projectName", out var pn) ? pn.GetString() ?? "" : ""
                });
            }
            return groups;
        }
        catch
        {
            return new List<GroupInfo>();
        }
    }

    private async Task<string?> GetLecturerNameAsync(Guid lecturerId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("IdentityService");
            var response = await client.GetAsync($"/api/lecturers/{lecturerId}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("value").GetProperty("data")
                .GetProperty("fullName").GetString();
        }
        catch
        {
            return null;
        }
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                    ?? User.FindFirst("sub")
                    ?? User.FindFirst("userId");
        if (claim != null && Guid.TryParse(claim.Value, out var id))
            return id;
        return Guid.Empty;
    }

    private static string NormalizeRole(string role) => role.Trim().ToLowerInvariant();

    private record SlotInfo { public Guid Id { get; init; } public Guid CampaignId { get; init; } }
    private record GroupInfo { public Guid Id { get; init; } public string GroupCode { get; init; } public string ProjectName { get; init; } }

    private class LecturerWorkloadDto
    {
        public Guid LecturerId { get; set; }
        public string LecturerName { get; set; } = "";
        public int TotalReviews { get; set; }
        public int PrimaryCount { get; set; }
        public int SecondaryCount { get; set; }
    }

    private class ConflictDto
    {
        public Guid? LecturerId { get; set; }
        public Guid? SlotId { get; set; }
        public string ConflictType { get; set; } = "";
        public string Message { get; set; } = "";
    }
}

public class ManualAssignRequest
{
    public Guid CapstoneGroupId { get; set; }
    public Guid ReviewSlotId { get; set; }
    public Guid PrimaryReviewerId { get; set; }
    public Guid SecondaryReviewerId { get; set; }
}
