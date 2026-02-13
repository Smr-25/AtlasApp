using FluentValidation;

namespace Atlas.Application.Features.SecurityTools.Queries.ScanVulnerabilities;

public class ScanVulnerabilitiesQueryValidator : AbstractValidator<ScanVulnerabilitiesQuery>
{
    public ScanVulnerabilitiesQueryValidator()
    {
        RuleFor(x => x.ProjectPath)
            .NotEmpty().WithMessage("Project path is required.")
            .Must(path => File.Exists(path) || Directory.Exists(path))
            .WithMessage("Invalid path. Please provide a valid .sln file or directory.");
    }
}