using Availability.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Availability.Application.Interfaces
{
    public interface ILecturerAvailabilityService
    {
        Task<LecturerAvailabilityDto?> CreateLecturerAvailabilityAsync(CreateLecturerAvailabilityDto request);
        Task<List<LecturerAvailabilityDto>> GetAllLecturerAvailabilitysAsync();
        Task<List<LecturerAvailabilityDto>> GetLecturerAvailabilityByLectureIdAsync(Guid id);
        Task<List<LecturerAvailabilityDto>> GetLecturerAvailabilityByReviewSlotIdAsync(Guid id);
        Task<LecturerAvailabilityDto?> GetLecturerAvailabilityByIdAsync(Guid id);
        Task<LecturerAvailabilityDto?> UpdateLecturerAvailabilityAsync(Guid id, UpdateLecturerAvailabilityDto request);
        Task<bool> DeleteLecturerAvailabilityAsync(Guid id);
    }
}
