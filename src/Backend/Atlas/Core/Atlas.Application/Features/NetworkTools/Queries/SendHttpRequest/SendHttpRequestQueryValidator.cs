using FluentValidation;
using FluentValidation.Validators;

namespace Atlas.Application.Features.NetworkTools.Queries.SendHttpRequest;

public class SendHttpRequestQueryValidator : AbstractValidator<SendHttpRequestQuery>
{
    public SendHttpRequestQueryValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("URL cannot be empty.")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Please enter a valid URL (starting with http:// or https://).");

        RuleFor(x => x.Method)
            .NotEmpty()
            .Must(method => new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "HEAD" }.Contains(method.ToUpper()))
            .WithMessage("Invalid HTTP Method. Allowed: GET, POST, PUT, DELETE, PATCH, HEAD.");

        When(x => x.Method.ToUpper() == "POST" || x.Method.ToUpper() == "PUT", () =>
        {
            RuleFor(x => x.Body)
                .NotEmpty().WithMessage("Body is required for POST/PUT requests.");
        });
    }
}