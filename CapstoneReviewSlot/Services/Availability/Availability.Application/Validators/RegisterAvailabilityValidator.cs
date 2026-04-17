using FluentValidation;
using Availability.Application.Features.Commands.RegisterAvailability;

namespace Availability.Application.Validators;

public class RegisterAvailabilityCommandValidator : AbstractValidator<RegisterAvailabilityCommand>
{
    public RegisterAvailabilityCommandValidator()
    {
        RuleFor(x => x.LecturerId)
            .GreaterThan(0).WithMessage("LecturerId phải là số dương.");

        RuleFor(x => x.SlotIds)
            .NotEmpty().WithMessage("Phải chọn ít nhất 1 slot.")
            .Must(ids => ids.All(id => id != Guid.Empty)).WithMessage("Tất cả SlotId phải là Guid hợp lệ.")
            .Must(ids => ids.Count <= 50).WithMessage("Không thể đăng ký quá 50 slot một lần.");
    }
}
