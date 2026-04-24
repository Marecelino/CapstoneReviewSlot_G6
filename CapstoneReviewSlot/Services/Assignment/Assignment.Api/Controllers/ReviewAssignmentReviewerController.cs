using Assignment.Domain.Dtos;
using Assignment.Domain.Interfaces.Services;
using Assignment.Domain.Ultils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Session.Application.Ultils;
using System.Text.Json;

namespace Assignment.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewAssignmentReviewerController : ControllerBase
    {
        private readonly IReviewAssignmentReviewerService _reviewAssignmentReviewerService;
        private readonly IReviewAssignmentService _reviewAssignmentService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ReviewAssignmentReviewerController(
            IReviewAssignmentReviewerService reviewAssignmentReviewerService,
            IReviewAssignmentService reviewAssignmentService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _reviewAssignmentReviewerService = reviewAssignmentReviewerService;
            _reviewAssignmentService = reviewAssignmentService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] List<ReviewAssignmentReviewerDto> request)
        {
            try
            {
                if (request == null || request.Count == 0)
                {
                    return BadRequest(ApiResult<object>.Failure("400", "Reviewer list is empty."));
                }

                var availabilityApiBaseUrl = _configuration["ServiceEndpoints:AvailabilityApi"]
                    ?? throw new InvalidOperationException("ServiceEndpoints:AvailabilityApi is not configured.");

                var client = _httpClientFactory.CreateClient();
                if (Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authHeader.ToString());
                }

                var allowedLecturersBySlot = new Dictionary<Guid, HashSet<Guid>>();

                foreach (var group in request.GroupBy(x => x.ReviewAssignmentId))
                {
                    var assignment = await _reviewAssignmentService.GetByIdAsync(group.Key);
                    var slotId = assignment.ReviewSlotId;

                    if (!allowedLecturersBySlot.TryGetValue(slotId, out var allowedLecturers))
                    {
                        var response = await client.GetAsync($"{availabilityApiBaseUrl}/api/Availability/slots/{slotId}");
                        if (!response.IsSuccessStatusCode)
                        {
                            return BadRequest(ApiResult<object>.Failure("400", $"Cannot validate lecturer availability for slot {slotId}."));
                        }

                        var json = await response.Content.ReadAsStringAsync();
                        using var doc = JsonDocument.Parse(json);

                        allowedLecturers = new HashSet<Guid>();
                        if (doc.RootElement.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var item in doc.RootElement.EnumerateArray())
                            {
                                if (!item.TryGetProperty("lecturerId", out var lecturerIdProp))
                                    continue;

                                if (!Guid.TryParse(lecturerIdProp.GetString(), out var lecturerId))
                                    continue;

                                // AvailabilityStatus.Available = 0
                                var isAvailable = false;
                                if (item.TryGetProperty("status", out var statusProp))
                                {
                                    if (statusProp.ValueKind == JsonValueKind.Number && statusProp.GetInt32() == 0)
                                        isAvailable = true;
                                    else if (statusProp.ValueKind == JsonValueKind.String &&
                                             string.Equals(statusProp.GetString(), "Available", StringComparison.OrdinalIgnoreCase))
                                        isAvailable = true;
                                }

                                if (isAvailable)
                                    allowedLecturers.Add(lecturerId);
                            }
                        }

                        allowedLecturersBySlot[slotId] = allowedLecturers;
                    }

                    foreach (var reviewer in group)
                    {
                        if (!allowedLecturers.Contains(reviewer.LecturerId))
                        {
                            return BadRequest(ApiResult<object>.Failure(
                                "400",
                                $"Lecturer {reviewer.LecturerId} is not available/registered for slot {slotId}."));
                        }
                    }
                }

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
