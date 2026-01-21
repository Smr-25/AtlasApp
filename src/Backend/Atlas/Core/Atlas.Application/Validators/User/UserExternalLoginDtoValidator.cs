using Atlas.Application.Dtos.Users;
using FluentValidation;

namespace Atlas.Application.Validators.User;

public class UserExternalLoginDtoValidator : AbstractValidator<UserExternalLoginDto>
{
    public UserExternalLoginDtoValidator()
    {
        RuleFor(x => x.Provider).NotEmpty().WithMessage("Provider is required.");
        RuleFor(x => x.IdToken).NotEmpty().WithMessage("IdToken is required.");
    }
}
