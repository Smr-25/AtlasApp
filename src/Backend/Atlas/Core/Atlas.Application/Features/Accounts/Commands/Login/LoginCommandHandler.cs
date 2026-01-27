using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Application.Services.Interfaces;
using Atlas.Application.Settings;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Atlas.Application.Features.Accounts.Commands.Login;

public class LoginCommandHandler(
    UserManager<AppUser> userManager,
    IJwtService jwtService,
    IOptions<LockoutSettings> options) : IRequestHandler<LoginCommand, ResponseModel<AuthResponseDto>>
{
    private readonly LockoutSettings _lockoutSettings = options.Value;

    public async Task<ResponseModel<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await FindUserByEmailOrUserNameAsync(request.Email, request.UserName);

        if (user == null)
            throw new InvalidCredentialsException("Invalid UserName/Email or Password.");

        if (user.IsDeleted)
            throw new UnauthorizedException("This account has been deleted.");

        if (user.IsLockedOut)
        {
            var remainingTime = user.LockoutEndTime!.Value - DateTime.UtcNow;
            throw new AccountLockedException(
                $"Account is locked. Try again in {(int)remainingTime.TotalMinutes} minutes.");
        }

        if (!user.EmailConfirmed)
            throw new EmailNotVerifiedException("Email is not verified. Please verify your email before logging in.");

        var passwordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            user.IncrementFailedLoginAttempts(
                _lockoutSettings.MaxFailedAccessAttempts,
                TimeSpan.FromMinutes(_lockoutSettings.LockoutDurationInMinutes));
            await userManager.UpdateAsync(user);

            if (user.IsLockedOut)
                throw new AccountLockedException(
                    $"Too many failed attempts. Account locked for {_lockoutSettings.LockoutDurationInMinutes} minutes.");

            var remainingAttempts = _lockoutSettings.MaxFailedAccessAttempts - user.FailedLoginAttempts;
            throw new InvalidCredentialsException(
                $"Invalid UserName/Email or Password. {remainingAttempts} attempts remaining before account lockout.");
        }

        user.ResetFailedLoginAttempts();
        await userManager.UpdateAsync(user);


        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = jwtService.GenerateRefreshTokenResponse(user);
        user.SetRefreshToken(refreshToken.RefreshToken, refreshToken.RefreshTokenExpiresAt);
        user.UpdateLastLogin();
        await userManager.UpdateAsync(user);
        var response = new AuthResponseDto(
            AccessToken: accessToken,
            RefreshToken: refreshToken.RefreshToken,
            AccessTokenExpiration: DateTime.UtcNow.AddMinutes(60),
            RefreshTokenExpiration: refreshToken.RefreshTokenExpiresAt,
            UserId: user.Id,
            UserName: user.UserName!,
            Email: user.Email!,
            FullName: user.FullName
        );

        return ResponseModel<AuthResponseDto>.Success(response);
    }

    private async Task<AppUser?> FindUserByEmailOrUserNameAsync(string? email, string? userName)
    {
        if (!string.IsNullOrEmpty(email))
            return await userManager.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (!string.IsNullOrEmpty(userName))
            return await userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);

        return null;
    }
}