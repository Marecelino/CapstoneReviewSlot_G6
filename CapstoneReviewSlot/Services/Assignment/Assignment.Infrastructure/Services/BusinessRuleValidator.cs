using Assignment.Application.Interfaces;
using Assignment.Domain.Dtos;
using Assignment.Domain.Interfaces.Repositories;
using Assignment.Domain.Ultils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Assignment.Infrastructure.Services;

public class BusinessRuleValidator : IBusinessRuleValidator
{
    private readonly IReviewAssignmentRepository _assignmentRepo;
    private readonly IReviewAssignmentReviewerRepository _reviewerRepo;
    private readonly IHttpClientFactory _httpClientFactory;

    public BusinessRuleValidator(
        IReviewAssignmentRepository assignmentRepo,
        IReviewAssignmentReviewerRepository reviewerRepo,
        IHttpClientFactory httpClientFactory)
    {
        _assignmentRepo = assignmentRepo;
        _reviewerRepo = reviewerRepo;
        _httpClientFactory = httpClientFactory;
    }

    public async Task ValidateAddReviewerAsync(ReviewAssignmentReviewerDto reviewer)
    {
        var assignment = await _assignmentRepo.GetByIdAsync(reviewer.ReviewAssignmentId);
        if (assignment == null) return;

        // Rule 1: Lecturer cannot review their own mentored group
        var group = await GetCapstoneGroupAsync(assignment.CapstoneGroupId);
        if (group != null && group.MentorLecturerId == reviewer.LecturerId)
        {
            throw ErrorHelper.Forbidden("Giảng viên không thể review nhóm mình hướng dẫn.");
        }

        // Rule 2: Lecturer must be available at the slot
        if (group != null)
        {
            var isAvailable = await CheckLecturerAvailabilityAsync(reviewer.LecturerId, group.ReviewSlotId);
            if (!isAvailable)
            {
                throw ErrorHelper.BadRequest("Giảng viên không available tại slot này.");
            }
        }

        // Rule 4: No duplicate reviewer in same slot across assignments
        var slotAssignments = await _assignmentRepo.GetBySlotId(assignment.ReviewSlotId);
        foreach (var slotAssignment in slotAssignments)
        {
            var existingReviewers = await _reviewerRepo.GetByReviewAssignmentIdAsync(slotAssignment.Id);
            if (existingReviewers.Any(r => r.LecturerId == reviewer.LecturerId))
            {
                throw ErrorHelper.Conflict("Giảng viên đã được assign vào slot này.");
            }
        }
    }

    public async Task ValidateAddAssignmentAsync(ReviewAssignmentRequest request)
    {
        var existedInSlot = await _assignmentRepo.GetBySlotId(request.ReviewSlotId);
        if (existedInSlot.Any(x => x.CapstoneGroupId == request.CapstoneGroupId))
        {
            throw ErrorHelper.Conflict("Nhóm đã được assign vào slot này rồi.");
        }

        var availableCount = await GetAvailableLecturerCountForSlotAsync(request.ReviewSlotId);
        if (availableCount < 2)
        {
            throw ErrorHelper.BadRequest("Slot này không đủ giảng viên available (cần ít nhất 2).");
        }
    }

    private async Task<CapstoneGroupInfo?> GetCapstoneGroupAsync(Guid groupId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("SessionService");
            var response = await client.GetAsync($"/api/groups/{groupId}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var data = doc.RootElement.GetProperty("value").GetProperty("data");

            return new CapstoneGroupInfo
            {
                Id = Guid.Parse(data.GetProperty("id").GetString()!),
                MentorLecturerId = data.TryGetProperty("mentorLecturerId", out var ml) && ml.ValueKind != JsonValueKind.Null
                    ? Guid.Parse(ml.GetString()!)
                    : Guid.Empty,
                ReviewSlotId = Guid.Empty
            };
        }
        catch { return null; }
    }

    private async Task<bool> CheckLecturerAvailabilityAsync(Guid lecturerId, Guid slotId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("AvailabilityService");
            var response = await client.GetAsync($"/api/availability/check?lecturerId={lecturerId}&slotId={slotId}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    private async Task<int> GetAvailableLecturerCountForSlotAsync(Guid slotId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("AvailabilityService");
            var response = await client.GetAsync($"/api/availability/slot/{slotId}/count");
            if (!response.IsSuccessStatusCode) return 0;

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("value").GetProperty("data")
                .GetProperty("count").GetInt32();
        }
        catch { return 0; }
    }

    private record CapstoneGroupInfo
    {
        public Guid Id { get; init; }
        public Guid MentorLecturerId { get; init; }
        public Guid ReviewSlotId { get; init; }
    }
}
