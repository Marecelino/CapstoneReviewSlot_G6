using Availability.Application.DTOs;
using Availability.Application.Features.Queries.GetSlotAvailability;
using Availability.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Availability.Application.Features.Queries.GetAllAvailability
{
    public record GetAllAvailabilityQuery() : IRequest<IEnumerable<LecturerAvailabilityDto>>;
    public class GetAllAvailabilityQueryHandler
    : IRequestHandler<GetSlotAvailabilityQuery, IEnumerable<LecturerAvailabilityDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetAllAvailabilityQueryHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<IEnumerable<LecturerAvailabilityDto>> Handle(
            GetSlotAvailabilityQuery request, CancellationToken ct)
        {
            var list = await _uow.Availabilities.GetAllAsync(ct);
            return list.Select(a => new LecturerAvailabilityDto(
                a.Id, a.LecturerId, a.ReviewSlotId, a.Status, a.RegisteredAt));
        }
    }
}
