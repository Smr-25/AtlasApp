using Atlas.Application.Dtos.Users.Profile;
using FluentValidation;

namespace Atlas.Application.Validators.User.Profile;

public class UserChangePasswordDtoValidator : AbstractValidator<UserChangePasswordDto>
{
    public UserChangePasswordDtoValidator()
    {
       
    }
}
