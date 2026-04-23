using Assignment.Domain.Dtos;

namespace Assignment.Application.Interfaces;

public interface IBusinessRuleValidator
{
    Task ValidateAddReviewerAsync(ReviewAssignmentReviewerDto reviewer);
    Task ValidateAddAssignmentAsync(ReviewAssignmentRequest request);
}
