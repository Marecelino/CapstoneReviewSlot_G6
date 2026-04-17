using Assignment.Domain.Dtos;
using Assignment.Domain.Interfaces.Services;
using Assignment.Domain.Ultils;
using Microsoft.AspNetCore.Mvc;
using Session.Application.Ultils;

namespace Assignment.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewAssignmentController : ControllerBase
    {
        private readonly IReviewAssignmentService _reviewAssignmentService;

        public ReviewAssignmentController(IReviewAssignmentService reviewAssignmentService)
        {
            _reviewAssignmentService = reviewAssignmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _reviewAssignmentService.GetAllAsync();
                return Ok(ApiResult<object>.Success(data, "200", "Get review assignments successfully."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<List<ReviewAssignmentDto>>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var data = await _reviewAssignmentService.GetByIdAsync(id);
                return Ok(ApiResult<object>.Success(data!, "200", "Get review assignment successfully."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<ReviewAssignmentDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpGet("by-slot/{slotId:guid}")]
        public async Task<IActionResult> GetBySlotId(Guid slotId)
        {
            try
            {
                var data = await _reviewAssignmentService.GetBySlotId(slotId);
                return Ok(ApiResult<object>.Success(data, "200", "Get review assignments by slot successfully."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<List<ReviewAssignmentDto>>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpGet("by-group/{groupId:guid}")]
        public async Task<IActionResult> GetByGroupId(Guid groupId)
        {
            try
            {
                var data = await _reviewAssignmentService.GetByGroupId(groupId);
                return Ok(ApiResult<object>.Success(data, "200", "Get review assignments by group successfully."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<List<ReviewAssignmentDto>>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReviewAssignmentRequest request)
        {
            try
            {
                var data = await _reviewAssignmentService.AddAsync(request);
                return Ok(ApiResult<object>.Success(data, "201", "Create review assignment successfully."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<ReviewAssignmentDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ReviewAssignmentRequest request)
        {
            try
            {
                var data = await _reviewAssignmentService.UpdateAsync(request);
                return Ok(ApiResult<object>.Success(data, "200", "Update review assignment successfully."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<ReviewAssignmentDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _reviewAssignmentService.DeleteAsync(id);
                return Ok(ApiResult<object>.Success(result, "200", "Delete review assignment successfully."));
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
