using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Session.Application.DTOs;
using Session.Application.Features.Queries.GetActiveCampaigns;
using Session.Application.Features.Queries.GetSlotsByCampaign;
using Session.Application.Features.Commands.CreateCampaign;
using Session.Application.Features.Commands.CreateSlots;

namespace Session.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignsController : ControllerBase
{
    private readonly IMediator _mediator;
    public CampaignsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Lấy danh sách campaign đang mở (trạng thái Open)</summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<ReviewCampaignDto>), 200)]
    public async Task<IActionResult> GetActive(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetActiveCampaignsQuery(), ct);
        return Ok(result);
    }

    /// <summary>Lấy danh sách slot của một campaign</summary>
    [HttpGet("{campaignId:guid}/slots")]
    [ProducesResponseType(typeof(IEnumerable<ReviewSlotDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetSlots(Guid campaignId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetSlotsByCampaignQuery(campaignId), ct);
        return Ok(result);
    }

    /// <summary>Tạo campaign mới — chỉ Admin/Manager</summary>
    [HttpPost]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(typeof(ReviewCampaignDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateCampaignCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetSlots), new { campaignId = result.CampaignId }, result);
    }

    /// <summary>Tạo batch slots cho campaign — chỉ Admin/Manager</summary>
    [HttpPost("{campaignId:guid}/slots")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(typeof(IEnumerable<ReviewSlotDto>), 201)]
    public async Task<IActionResult> CreateSlots(
        Guid campaignId, [FromBody] CreateSlotsCommand command, CancellationToken ct)
    {
        if (campaignId != command.CampaignId)
            return BadRequest("CampaignId trong URL và body phải khớp nhau.");

        var result = await _mediator.Send(command, ct);
        return StatusCode(201, result);
    }
}
