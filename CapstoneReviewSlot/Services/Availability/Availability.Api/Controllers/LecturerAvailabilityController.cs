using Availability.Application.Interfaces;
using Availability.Application.Ultils;
using Availability.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Availability.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LecturerAvailabilityController : ControllerBase
    {
        private readonly ILecturerAvailabilityService _service;
        public LecturerAvailabilityController(ILecturerAvailabilityService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var reviewSlot = await _service.GetAllLecturerAvailabilitysAsync();
                return Ok(ApiResult<object>.Success(reviewSlot!, "200", "Get Successfully!"));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<LecturerAvailabilityDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var reviewSlot = await _service.GetLecturerAvailabilityByIdAsync(id);
                return Ok(ApiResult<object>.Success(reviewSlot!, "200", "Get Successfully!"));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<LecturerAvailabilityDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpGet("lecture/{lectureId}")]
        public async Task<IActionResult> GetByLectureId(Guid lectureId)
        {
            try
            {
                var reviewSlot = await _service.GetLecturerAvailabilityByLectureIdAsync(lectureId);
                return Ok(ApiResult<object>.Success(reviewSlot!, "200", "Get Successfully!"));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<LecturerAvailabilityDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpGet("slot/{slotId}")]
        public async Task<IActionResult> GetBySlotReviewId(Guid slotId)
        {
            try
            {
                var reviewSlot = await _service.GetLecturerAvailabilityByReviewSlotIdAsync(slotId);
                return Ok(ApiResult<object>.Success(reviewSlot!, "200", "Get Successfully!"));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<LecturerAvailabilityDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAvailable([FromBody] CreateLecturerAvailabilityDto request)
        {
            try
            {
                var updatedReviewSlot = await _service.CreateLecturerAvailabilityAsync(request);
                return Ok(ApiResult<object>.Success(updatedReviewSlot!, "200", "Create Successfully!"));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<LecturerAvailabilityDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAvailable(Guid id, [FromBody] UpdateLecturerAvailabilityDto request)
        {
            try
            {
                var updatedReviewSlot = await _service.UpdateLecturerAvailabilityAsync(id, request);
                return Ok(ApiResult<object>.Success(updatedReviewSlot!, "200", "Update Successfully!"));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<LecturerAvailabilityDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReviewSlot(Guid id)
        {
            try
            {
                var result = await _service.DeleteLecturerAvailabilityAsync(id);
                return Ok(ApiResult<object>.Success(result, "200", "Delete Successfully!"));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<bool>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }
    }
}
