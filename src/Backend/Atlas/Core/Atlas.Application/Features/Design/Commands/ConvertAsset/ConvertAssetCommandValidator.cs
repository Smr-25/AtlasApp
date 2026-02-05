using FluentValidation;

namespace Atlas.Application.Features.Design.Commands.ConvertAsset;

public class ConvertAssetCommandValidator : AbstractValidator<ConvertAssetCommand>
{
    public ConvertAssetCommandValidator()
    {
        RuleFor(x => x.File).NotNull().WithMessage("File is required.");
        RuleFor(x => x.TargetFormat)
            .Must(x => new[] { "webp", "png", "jpg", "jpeg" }.Contains(x.ToLower()))
            .WithMessage("Supported formats: webp, png, jpg.");
    }
}