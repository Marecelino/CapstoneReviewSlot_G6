using Availability.Application.DTOs;
using Availability.Domain.Interfaces;
using MediatR;

namespace Availability.Application.Features.Queries.GetMyAvailability;

/// <summary>Giảng viên xem lịch rảnh đã đăng ký của chính mình</summary>
public record GetMyAvailabilityQuery(Guid LecturerId) : IRequest<IEnumerable<LecturerAvailabilityDto>>;

public class GetMyAvailabilityQueryHandler
    : IRequestHandler<GetMyAvailabilityQuery, IEnumerable<LecturerAvailabilityDto>>
{
    private readonly IUnitOfWork _uow;
    public GetMyAvailabilityQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<LecturerAvailabilityDto>> Handle(
        GetMyAvailabilityQuery request, CancellationToken ct)
    {
        var list = await _uow.Availabilities.GetByLecturerIdAsync(request.LecturerId, ct);
        return list.Select(a => new LecturerAvailabilityDto(
            a.Id, a.LecturerId, a.ReviewSlotId, a.Status, a.RegisteredAt));
    }
}
