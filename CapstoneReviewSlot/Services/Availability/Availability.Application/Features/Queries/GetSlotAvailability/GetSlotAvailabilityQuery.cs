using Availability.Application.DTOs;
using Availability.Domain.Interfaces;
using MediatR;

namespace Availability.Application.Features.Queries.GetSlotAvailability;

/// <summary>Admin/Manager xem tất cả giảng viên đăng ký một slot</summary>
public record GetSlotAvailabilityQuery(Guid ReviewSlotId) : IRequest<IEnumerable<LecturerAvailabilityDto>>;

public class GetSlotAvailabilityQueryHandler
    : IRequestHandler<GetSlotAvailabilityQuery, IEnumerable<LecturerAvailabilityDto>>
{
    private readonly IUnitOfWork _uow;
    public GetSlotAvailabilityQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<LecturerAvailabilityDto>> Handle(
        GetSlotAvailabilityQuery request, CancellationToken ct)
    {
        var list = await _uow.Availabilities.GetBySlotIdAsync(request.ReviewSlotId, ct);
        return list.Select(a => new LecturerAvailabilityDto(
            a.Id, a.LecturerId, a.ReviewSlotId, a.Status, a.RegisteredAt));
    }
}
