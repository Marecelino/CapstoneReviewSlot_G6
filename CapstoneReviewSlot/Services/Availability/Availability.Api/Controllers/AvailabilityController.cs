using Availability.Application.DTOs;
using Availability.Application.Features.Commands.CancelAvailability;
using Availability.Application.Features.Commands.RegisterAvailability;
using Availability.Application.Features.Queries.GetMyAvailability;
using Availability.Application.Features.Queries.GetSlotAvailability;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Availability.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AvailabilityController : ControllerBase
{
    private readonly IMediator _mediator;

    public AvailabilityController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// [Lecturer] Đăng ký danh sách slot rảnh.
    /// LecturerId tự động lấy từ JWT — không cần truyền lên.
    /// </summary>
    [HttpPost("register")]
    [Authorize(Roles = "Lecturer")]
    [ProducesResponseType(typeof(IEnumerable<LecturerAvailabilityDto>), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterAvailabilityRequest request, CancellationToken ct)
    {
        var lecturerId = GetLecturerId();
        if (lecturerId is null)
            return Unauthorized("Không tìm thấy LecturerId trong token. Vui lòng đăng nhập lại.");

        var command = new RegisterAvailabilityCommand(lecturerId.Value, request.SlotIds);
        var result = await _mediator.Send(command, ct);
        return StatusCode(201, result);
    }

    /// <summary>
    /// [Lecturer] Xem tất cả slot đã đăng ký của bản thân
    /// </summary>
    [HttpGet("my")]
    [Authorize(Roles = "Lecturer")]
    [ProducesResponseType(typeof(IEnumerable<LecturerAvailabilityDto>), 200)]
    public async Task<IActionResult> GetMine(CancellationToken ct)
    {
        var lecturerId = GetLecturerId();
        if (lecturerId is null) return Unauthorized();

        var result = await _mediator.Send(new GetMyAvailabilityQuery(lecturerId.Value), ct);
        return Ok(result);
    }

    /// <summary>
    /// [Lecturer] Huỷ đăng ký một slot
    /// </summary>
    [HttpDelete("slots/{slotId:guid}")]
    [Authorize(Roles = "Lecturer")]
    [ProducesResponseType(typeof(LecturerAvailabilityDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Cancel(Guid slotId, CancellationToken ct)
    {
        var lecturerId = GetLecturerId();
        if (lecturerId is null) return Unauthorized();

        var result = await _mediator.Send(new CancelAvailabilityCommand(lecturerId.Value, slotId), ct);
        return Ok(result);
    }

    /// <summary>
    /// [Admin] Xem tất cả giảng viên đã đăng ký một slot cụ thể
    /// </summary>
    [HttpGet("slots/{slotId:guid}")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(typeof(IEnumerable<LecturerAvailabilityDto>), 200)]
    public async Task<IActionResult> GetBySlot(Guid slotId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetSlotAvailabilityQuery(slotId), ct);
        return Ok(result);
    }

    // ── Helper ────────────────────────────────────────────────────────────────
    private Guid? GetLecturerId()
    {
        var lecturerId = User.FindFirst("lecturer_id")?.Value;

        if (!Guid.TryParse(lecturerId, out var id))
            return null;

        return id;
    }
}
