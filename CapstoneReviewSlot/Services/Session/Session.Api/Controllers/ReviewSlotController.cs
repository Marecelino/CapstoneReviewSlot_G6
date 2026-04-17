using Microsoft.AspNetCore.Mvc;
using Session.Application.Interfaces;
using Session.Application.Ultils;
using Session.Domain.DTOs;

namespace Session.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewSlotController : ControllerBase
    {
        private readonly IReviewSlotService _reviewSlotService;
        public ReviewSlotController(IReviewSlotService reviewSlotService)
        {
            _reviewSlotService = reviewSlotService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewSlotById(Guid id)
        {
            try
            {
                var reviewSlot = await _reviewSlotService.GetReviewSlotByIdAsync(id);
                return Ok(ApiResult<object>.Success(reviewSlot!, "200", "Get Review Slot Successfully!"));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<ReviewSlotDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpGet("by-date")]
        public async Task<IActionResult> GetReviewSlotsByDate([FromQuery] DateOnly date)
        {
            try
            {
                var reviewSlots = await _reviewSlotService.GetReviewSlotsByDate(date);
                return Ok(ApiResult<object>.Success(reviewSlots, "200", "Get Review Slots By Date Successfully!"));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<List<ReviewSlotDto>>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpPut("{id}/update-room")]
        public async Task<IActionResult> UpdateReviewSlotRoom(Guid id, [FromBody] string request)
        {
            try
            {
                var updatedReviewSlot = await _reviewSlotService.UpdateReviewSlotRoom(id, request);
                return Ok(ApiResult<object>.Success(updatedReviewSlot!, "200", "Update Review Slot Room Successfully!"));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<ReviewSlotDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReviewSlot(Guid id)
        {
            try
            {
                var result = await _reviewSlotService.DeleteReviewSlotAsync(id);
                return Ok(ApiResult<object>.Success(result, "200", "Delete Review Slot Successfully!"));
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
