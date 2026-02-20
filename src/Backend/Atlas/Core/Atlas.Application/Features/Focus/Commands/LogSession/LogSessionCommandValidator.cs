using FluentValidation;

namespace Atlas.Application.Features.Focus.Commands.LogSession;

public class LogSessionCommandValidator : AbstractValidator<LogSessionCommand>
{
    public LogSessionCommandValidator()
    {
        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Duration must be greater than 0.")
            .LessThanOrEqualTo(480).WithMessage("Duration cannot exceed 8 hours (480 minutes).");
        RuleFor(x => x.Tag)
            .NotEmpty().WithMessage("Tag is required.")
            .MaximumLength(50).WithMessage("Tag must not exceed 50 characters.");
        RuleFor(x => x.SessionType)
            .IsInEnum().WithMessage("Invalid session type.");
        RuleFor(x => x.BreakDurationMinutes)
            .GreaterThanOrEqualTo(0).WithMessage("Break duration cannot be negative.")
            .LessThanOrEqualTo(60).WithMessage("Break duration cannot exceed 60 minutes.");
    }
}

