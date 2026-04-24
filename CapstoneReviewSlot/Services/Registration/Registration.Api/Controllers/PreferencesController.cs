using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registration.Application.Interfaces;
using Registration.Application.Ultils;
using Registration.Domain.DTOs;

namespace Registration.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PreferencesController : ControllerBase
{
    private readonly IStudentSlotPreferenceService _service;

    public PreferencesController(IStudentSlotPreferenceService service) => _service = service;

    /// <summary>Register a student's slot preference.</summary>
    [HttpPost]
    [Authorize(Roles = "Student,Admin,Manager")]
    [ProducesResponseType(typeof(StudentSlotPreferenceDto), 201)]
    public async Task<IActionResult> Register(
        [FromBody] CreateStudentSlotPreferenceDto dto,
        CancellationToken ct)
    {
        try
        {
            var result = await _service.RegisterPreferenceAsync(dto, ct);
            return CreatedAtAction(nameof(GetByGroup), new { groupId = dto.CapstoneGroupId }, result);
        }
        catch (Exception ex)
        {
            return GetErrorResult(ex);
        }
    }

    /// <summary>Cancel a preference registration.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Student,Admin,Manager")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        try
        {
            await _service.CancelPreferenceAsync(id, ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            return GetErrorResult(ex);
        }
    }

    /// <summary>Get preferences for a capstone group.</summary>
    [HttpGet("group/{groupId:guid}")]
    [ProducesResponseType(typeof(List<StudentSlotPreferenceDto>), 200)]
    public async Task<IActionResult> GetByGroup(Guid groupId, CancellationToken ct)
    {
        try
        {
            var result = await _service.GetByGroupAsync(groupId, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return GetErrorResult(ex);
        }
    }

    /// <summary>Get preferences for a review slot.</summary>
    [HttpGet("slot/{slotId:guid}")]
    [ProducesResponseType(typeof(List<StudentSlotPreferenceDto>), 200)]
    public async Task<IActionResult> GetBySlot(Guid slotId, CancellationToken ct)
    {
        try
        {
            var result = await _service.GetBySlotAsync(slotId, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return GetErrorResult(ex);
        }
    }

    private IActionResult GetErrorResult(Exception ex)
    {
        var code = ex.Data.Contains("StatusCode") ? (int)ex.Data["StatusCode"]! : 500;
        return StatusCode(code, new { error = ex.Message });
    }
}
