using Microsoft.AspNetCore.Mvc;
using Session.Application.Interfaces;
using Session.Application.Ultils;
using Session.Domain.DTOs;
using Session.Domain.Enums;

namespace Session.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewCampaignController : ControllerBase
    {
        private readonly IReviewCampaignService _reviewCampaignService;

        public ReviewCampaignController(IReviewCampaignService reviewCampaignService)
        {
            _reviewCampaignService = reviewCampaignService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReviewCampaign([FromBody] CreateReviewCampaignDto request)
        {
            try
            {
                var result = await _reviewCampaignService.CreateReviewCampaignAsync(request);
                return Ok(ApiResult<object>.Success(result!, "200", "Create Review Campaign Successfully!"));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<ReviewCampaignDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviewCampaigns()
        {
            try
            {
                var result = await _reviewCampaignService.GetAllReviewCampaignsAsync();
                return Ok(ApiResult<List<ReviewCampaignDto>>.Success(result, "200", "Get All Review Campaigns Successfully!"));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<List<ReviewCampaignDto>>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewCampaignById(Guid id)
        {
            try
            {
                var result = await _reviewCampaignService.GetReviewCampaignByIdAsync(id);
                return Ok(ApiResult<ReviewCampaignDto>.Success(result!, "200", "Get Review Campaign By Id Successfully!"));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<ReviewCampaignDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReviewCampaign(Guid id, [FromBody] UpdateReviewCampaignDto request)
        {
            try
            {
                var result = await _reviewCampaignService.UpdateReviewCampaignAsync(id, request);
                return Ok(ApiResult<ReviewCampaignDto>.Success(result!, "200", "Update Review Campaign Successfully!"));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<ReviewCampaignDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeReviewCampaignStatus(Guid id, [FromQuery] ReviewCampaignStatus status)
        {
            try
            {
                var result = await _reviewCampaignService.ChangeReviewCampaignStatusAsync(id, status);
                if (!result)
                {
                    return NotFound(ApiResult<object>.Failure("404", "Review Campaign not found!"));
                }
                return Ok(ApiResult<object>.Success(null!, "200", "Change Review Campaign Status Successfully!"));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<ReviewCampaignDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReviewCampaign(Guid id)
        {
            try
            {
                var result = await _reviewCampaignService.DeleteReviewCampaignAsync(id);
                if (!result)
                {
                    return NotFound(ApiResult<object>.Failure("404", "Review Campaign not found!"));
                }
                return Ok(ApiResult<object>.Success(null!, "200", "Delete Review Campaign Successfully!"));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<ReviewCampaignDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }
    }
}
