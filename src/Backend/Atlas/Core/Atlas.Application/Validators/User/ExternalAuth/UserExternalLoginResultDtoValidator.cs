using Atlas.Application.Dtos.Users.ExternalAuth;
using FluentValidation;

namespace Atlas.Application.Validators.User.ExternalAuth;

public class UserExternalLoginResultDtoValidator : AbstractValidator<UserExternalLoginResultDto>
{
    public UserExternalLoginResultDtoValidator()
    {
        RuleFor(x=>x.AccessToken).NotEmpty().NotNull();
        RuleFor(x=>x.RefreshToken).NotEmpty().NotNull();
        RuleFor(x=>x.IsNewUser).NotNull();
    }   
}
