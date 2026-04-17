using MediatR;
using Session.Application.DTOs;
using Session.Domain.Entities;
using Session.Domain.Interfaces;

namespace Session.Application.Features.Commands.CreateSlots;

/// <summary>Tạo nhiều ReviewSlot trong một Campaign theo batch</summary>
public record SlotInput(
    DateOnly ReviewDate,
    int SlotNumber,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int MaxCapacity,
    string? Room = null);

public record CreateSlotsCommand(Guid CampaignId, IReadOnlyList<SlotInput> Slots)
    : IRequest<IEnumerable<ReviewSlotDto>>;

public class CreateSlotsCommandHandler
    : IRequestHandler<CreateSlotsCommand, IEnumerable<ReviewSlotDto>>
{
    private readonly IUnitOfWork _uow;
    public CreateSlotsCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<ReviewSlotDto>> Handle(CreateSlotsCommand req, CancellationToken ct)
    {
        var campaign = await _uow.Campaigns.GetByIdAsync(req.CampaignId, ct)
            ?? throw new KeyNotFoundException($"Campaign {req.CampaignId} không tồn tại.");

        var slots = new List<ReviewSlot>();
        foreach (var input in req.Slots)
        {
            if (await _uow.Slots.ExistsAsync(req.CampaignId, input.ReviewDate, input.SlotNumber, ct))
                throw new InvalidOperationException(
                    $"Slot #{input.SlotNumber} ngày {input.ReviewDate} đã tồn tại trong campaign này.");

            slots.Add(ReviewSlot.Create(
                req.CampaignId, input.ReviewDate, input.SlotNumber,
                input.StartTime, input.EndTime, input.MaxCapacity, input.Room));
        }

        await _uow.Slots.AddRangeAsync(slots, ct);
        await _uow.SaveChangesAsync(ct);

        return slots.Select(s => new ReviewSlotDto(
            s.Id, s.CampaignId, s.ReviewDate, s.SlotNumber,
            s.StartTime, s.EndTime, s.Room, s.MaxCapacity));
    }
}
