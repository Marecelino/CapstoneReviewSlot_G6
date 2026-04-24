using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Session.Application.Interfaces;
using Session.Domain.DTOs;

namespace Session.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly ICapstoneGroupService _groupService;
    private readonly IExcelImportService _importService;

    public GroupsController(ICapstoneGroupService groupService, IExcelImportService importService)
    {
        _groupService = groupService;
        _importService = importService;
    }

    /// <summary>Import groups + students from SE_CapstoneProject FA25 Excel file.</summary>
    [HttpPost("campaign/{campaignId:guid}/import")]
    [Authorize(Roles = "Admin,Manager,Lecturer")]
    [ProducesResponseType(typeof(ImportResultDto), 200)]
    public async Task<IActionResult> ImportGroups(
        Guid campaignId,
        IFormFile file,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "File is required." });

        if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { error = "Only .xlsx files are supported." });

        await using var stream = file.OpenReadStream();
        var result = await _importService.ImportGroupsAndStudentsAsync(
            campaignId, stream, file.FileName, ct);
        return Ok(result);
    }

    /// <summary>Import lecturer availability from LecturerBookingReviewSlots Excel file.</summary>
    [HttpPost("availability/import")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ImportResultDto), 200)]
    public async Task<IActionResult> ImportAvailability(
        IFormFile file,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "File is required." });

        if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { error = "Only .xlsx files are supported." });

        await using var stream = file.OpenReadStream();
        var result = await _importService.ImportAvailabilityAsync(stream, file.FileName, ct);
        return Ok(result);
    }

    /// <summary>Get all groups in a campaign.</summary>
    [HttpGet("campaign/{campaignId:guid}")]
    [ProducesResponseType(typeof(List<CapstoneGroupDto>), 200)]
    public async Task<IActionResult> GetByCampaign(Guid campaignId, CancellationToken ct)
    {
        var groups = await _groupService.GetByCampaignAsync(campaignId, ct);
        return Ok(groups);
    }

    /// <summary>Get a group by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CapstoneGroupDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var group = await _groupService.GetByIdAsync(id, ct);
        if (group is null) return NotFound();
        return Ok(group);
    }

    /// <summary>Get a group by code (e.g. GFA25SE01).</summary>
    [HttpGet("code/{groupCode}")]
    [ProducesResponseType(typeof(CapstoneGroupDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByCode(string groupCode, CancellationToken ct)
    {
        var group = await _groupService.GetByGroupCodeAsync(groupCode, ct);
        if (group is null) return NotFound();
        return Ok(group);
    }

    /// <summary>Create a single group manually.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(CapstoneGroupDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateCapstoneGroupDto dto, CancellationToken ct)
    {
        var group = await _groupService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = group.Id }, group);
    }

    /// <summary>Delete a group (soft delete).</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _groupService.DeleteAsync(id, ct);
        return NoContent();
    }

    /// <summary>Get group count for a campaign.</summary>
    [HttpGet("campaign/{campaignId:guid}/count")]
    [ProducesResponseType(typeof(int), 200)]
    public async Task<IActionResult> CountByCampaign(Guid campaignId, CancellationToken ct)
    {
        var count = await _groupService.CountByCampaignAsync(campaignId, ct);
        return Ok(count);
    }
}
