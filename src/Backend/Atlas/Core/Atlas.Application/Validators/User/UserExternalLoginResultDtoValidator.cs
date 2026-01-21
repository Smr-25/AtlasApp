using Atlas.Application.Dtos.Users;
using FluentValidation;

namespace Atlas.Application.Validators.User;

public class UserExternalLoginResultDtoValidator : AbstractValidator<UserExternalLoginResultDto>
{
    public UserExternalLoginResultDtoValidator()
    {
        RuleFor(x=>x.AccessToken).NotEmpty().NotNull();
        RuleFor(x=>x.RefreshToken).NotEmpty().NotNull();
        RuleFor(x=>x.IsNewUser).NotNull();
    }   
}
