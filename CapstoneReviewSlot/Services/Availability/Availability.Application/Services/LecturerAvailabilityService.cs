using Availability.Application.Interfaces;
using Availability.Application.Ultils;
using Availability.Domain.Dtos;
using Availability.Domain.Entities;
using Availability.Infrastructure.Common;
using Availability.Infrastructure.Interfaces;

namespace Availability.Application.Services
{
    public class LecturerAvailabilityService : ILecturerAvailabilityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LecturerAvailabilityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LecturerAvailabilityDto?> CreateLecturerAvailabilityAsync(CreateLecturerAvailabilityDto request)
        {
            if (request == null)
            {
                throw ErrorHelper.BadRequest("Invalid data!");
            }
            var availability = await _unitOfWork.LecturerAvailabilities.AddAsync(new LecturerAvailability
            {
                LecturerId = request.LecturerId,
                ReviewSlotId = request.ReviewSlotId,
                CreatedAtUtc = DateTime.UtcNow
            });
            await _unitOfWork.SaveChangesAsync();
            return ToLecturerAvailabilityDto(availability);
        }

        public async Task<bool> DeleteLecturerAvailabilityAsync(Guid id)
        {
            var availability = await _unitOfWork.LecturerAvailabilities.GetByIdAsync(id);

            if (availability == null)
            {
                throw ErrorHelper.NotFound("Data not found!");
            }

            var isDeleted = await _unitOfWork.LecturerAvailabilities.HardRemoveAsync(availability);
            if (!isDeleted)
            {
                throw ErrorHelper.BadRequest("Delete failed!");
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<List<LecturerAvailabilityDto>> GetAllLecturerAvailabilitysAsync()
        {
            var availabilities = await _unitOfWork.LecturerAvailabilities.GetAllAsync();
            if (!availabilities.Any())
            {
                throw ErrorHelper.NotFound("Data not found!");
            }
            return availabilities.Select(a => new LecturerAvailabilityDto
            {
                Id = a.Id,
                ReviewSlotId = a.ReviewSlotId,
                LecturerId = a.LecturerId,
                CreatedAtUtc = a.CreatedAtUtc,
                UpdatedAtUtc = a.UpdatedAtUtc

            }).ToList();
        }

        public async Task<LecturerAvailabilityDto?> GetLecturerAvailabilityByIdAsync(Guid id)
        {
            var availability = await _unitOfWork.LecturerAvailabilities.GetByIdAsync(id);
            if (availability == null)
            {
                throw ErrorHelper.NotFound("Data not found!");
            }
            return ToLecturerAvailabilityDto(availability);
        }

        public async Task<List<LecturerAvailabilityDto>> GetLecturerAvailabilityByLectureIdAsync(Guid id)
        {
            var availabilities = await _unitOfWork.LecturerAvailabilities.GetByLecturerIdAsync(id);
            if (!availabilities.Any())
            {
                throw ErrorHelper.NotFound("Data not found!");
            }
            return availabilities.Select(a => new LecturerAvailabilityDto
            {
                Id = a.Id,
                ReviewSlotId = a.ReviewSlotId,
                LecturerId = a.LecturerId,
                CreatedAtUtc = a.CreatedAtUtc,
                UpdatedAtUtc = a.UpdatedAtUtc

            }).ToList();
        }

        public async Task<List<LecturerAvailabilityDto>> GetLecturerAvailabilityByReviewSlotIdAsync(Guid id)
        {
            var availabilities = await _unitOfWork.LecturerAvailabilities.GetByReviewSlotIdAsync(id);
            if (availabilities == null)
            {
                throw ErrorHelper.NotFound("Data not found!");
            }
            return availabilities.Select(a => new LecturerAvailabilityDto
            {
                Id = a.Id,
                ReviewSlotId = a.ReviewSlotId,
                LecturerId = a.LecturerId,
                CreatedAtUtc = a.CreatedAtUtc,
                UpdatedAtUtc = a.UpdatedAtUtc

            }).ToList();
        }

        public async Task<LecturerAvailabilityDto?> UpdateLecturerAvailabilityAsync(Guid id, UpdateLecturerAvailabilityDto request)
        {
            var availability = await _unitOfWork.LecturerAvailabilities.GetByIdAsync(id);

            if (availability == null)
                throw ErrorHelper.NotFound("Data not found!");

            availability.LecturerId = request.LecturerId;
            availability.ReviewSlotId = request.ReviewSlotId;
            availability.UpdatedAtUtc = DateTime.UtcNow;

            await _unitOfWork.LecturerAvailabilities.Update(availability);
            await _unitOfWork.SaveChangesAsync();
            return ToLecturerAvailabilityDto(availability);
        }

        private LecturerAvailabilityDto ToLecturerAvailabilityDto(LecturerAvailability availability)
        {
            return new LecturerAvailabilityDto
            {
                Id = availability.Id,
                ReviewSlotId = availability.ReviewSlotId,
                LecturerId = availability.LecturerId,
                CreatedAtUtc = availability.CreatedAtUtc,
                UpdatedAtUtc = availability.UpdatedAtUtc

            };
        }
    }
}
