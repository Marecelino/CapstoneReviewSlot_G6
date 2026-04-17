using Availability.Application.DTOs;
using Availability.Domain.Interfaces;
using MediatR;

namespace Availability.Application.Features.Commands.CancelAvailability;

public record CancelAvailabilityCommand(Guid LecturerId, Guid SlotId) : IRequest<LecturerAvailabilityDto>;

public class CancelAvailabilityCommandHandler
    : IRequestHandler<CancelAvailabilityCommand, LecturerAvailabilityDto>
{
    private readonly IUnitOfWork _uow;
    public CancelAvailabilityCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<LecturerAvailabilityDto> Handle(CancelAvailabilityCommand request, CancellationToken ct)
    {
        var record = await _uow.Availabilities.GetByLecturerAndSlotAsync(
            request.LecturerId, request.SlotId, ct)
            ?? throw new KeyNotFoundException(
                $"Giảng viên {request.LecturerId} chưa đăng ký slot {request.SlotId}.");

        await _uow.Availabilities.DeleteAsync(record, ct);
        await _uow.SaveChangesAsync(ct);

        return new LecturerAvailabilityDto(
            record.Id, record.LecturerId, record.ReviewSlotId, record.Status, record.RegisteredAt);
    }
}
