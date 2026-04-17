using Assignment.Domain.Dtos;
using Assignment.Domain.Interfaces.Services;
using Assignment.Domain.Ultils;
using Microsoft.AspNetCore.Mvc;
using Session.Application.Ultils;

namespace Assignment.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewAssignmentReviewerController : ControllerBase
    {
        private readonly IReviewAssignmentReviewerService _reviewAssignmentReviewerService;

        public ReviewAssignmentReviewerController(IReviewAssignmentReviewerService reviewAssignmentReviewerService)
        {
            _reviewAssignmentReviewerService = reviewAssignmentReviewerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _reviewAssignmentReviewerService.GetAllAsync();
                return Ok(ApiResult<object>.Success(data, "200", "Get review assignment reviewers successfully."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<List<ReviewAssignmentReviewerDto>>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var data = await _reviewAssignmentReviewerService.GetByIdAsync(id);
                return Ok(ApiResult<object>.Success(data, "200", "Get review assignment reviewer successfully."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<ReviewAssignmentReviewerDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpGet("by-assignment/{reviewAssignmentId:guid}")]
        public async Task<IActionResult> GetByReviewAssignmentId(Guid reviewAssignmentId)
        {
            try
            {
                var data = await _reviewAssignmentReviewerService.GetByReviewAssignmentIdAsync(reviewAssignmentId);
                return Ok(ApiResult<object>.Success(data, "200", "Get reviewers by review assignment successfully."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<List<ReviewAssignmentReviewerDto>>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpGet("by-lecturer/{lecturerId:guid}")]
        public async Task<IActionResult> GetByLecturerId(Guid lecturerId)
        {
            try
            {
                var data = await _reviewAssignmentReviewerService.GetByLecturerIdAsync(lecturerId);
                return Ok(ApiResult<object>.Success(data, "200", "Get review assignments by lecturer successfully."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<List<ReviewAssignmentReviewerDto>>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] List<ReviewAssignmentReviewerDto> request)
        {
            try
            {
                var data = await _reviewAssignmentReviewerService.AddAsync(request);
                return Ok(ApiResult<object>.Success(data, "201", "Create review assignment reviewers successfully."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<List<ReviewAssignmentReviewerDto>>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ReviewAssignmentReviewerDto request)
        {
            try
            {
                var data = await _reviewAssignmentReviewerService.UpdateAsync(id, request);
                return Ok(ApiResult<object>.Success(data, "200", "Update review assignment reviewer successfully."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<ReviewAssignmentReviewerDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _reviewAssignmentReviewerService.DeleteAsync(id);
                return Ok(ApiResult<object>.Success(result, "200", "Delete review assignment reviewer successfully."));
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
