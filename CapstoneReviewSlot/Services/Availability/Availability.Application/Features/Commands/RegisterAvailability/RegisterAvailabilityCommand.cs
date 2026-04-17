using Availability.Application.DTOs;
using Availability.Domain.Entities;
using Availability.Domain.Interfaces;
using MediatR;

namespace Availability.Application.Features.Commands.RegisterAvailability;

/// <summary>
/// Command: Giảng viên đăng ký danh sách slot rảnh.
/// LecturerId được trích xuất từ JWT (không nhận từ body).
/// </summary>
public record RegisterAvailabilityCommand(
    Guid LecturerId,
    IReadOnlyList<Guid> SlotIds) : IRequest<IEnumerable<LecturerAvailabilityDto>>;

public class RegisterAvailabilityCommandHandler
    : IRequestHandler<RegisterAvailabilityCommand, IEnumerable<LecturerAvailabilityDto>>
{
    private readonly IUnitOfWork _uow;

    public RegisterAvailabilityCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<LecturerAvailabilityDto>> Handle(
        RegisterAvailabilityCommand request, CancellationToken ct)
    {
        if (request.SlotIds is null || !request.SlotIds.Any())
            throw new ArgumentException("Phải chọn ít nhất 1 slot.");

        var toAdd = new List<LecturerAvailability>();

        foreach (var slotId in request.SlotIds.Distinct())
        {
            // Bỏ qua nếu đã đăng ký slot này trước đó (idempotent)
            if (await _uow.Availabilities.ExistsAsync(request.LecturerId, slotId, ct))
                continue;

            toAdd.Add(LecturerAvailability.Create(request.LecturerId, slotId));
        }

        if (toAdd.Any())
        {
            await _uow.Availabilities.AddRangeAsync(toAdd, ct);
            await _uow.SaveChangesAsync(ct);
        }

        // Trả về toàn bộ danh sách hiện tại của giảng viên này (bao gồm cũ + mới)
        var all = await _uow.Availabilities.GetByLecturerIdAsync(request.LecturerId, ct);
        return all.Select(a => new LecturerAvailabilityDto(
            a.Id, a.LecturerId, a.ReviewSlotId, a.Status, a.RegisteredAt));
    }
}
