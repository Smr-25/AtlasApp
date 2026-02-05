using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Application.Features.Accounts.Commands.ExternalLogin;

public class ExternalLoginCommandHandler(
    UserManager<AppUser> userManager,
    IJwtService jwtService,
    IExternalAuthService externalAuthService)
    : IRequestHandler<ExternalLoginCommand, ExternalLoginResponseDto>
{
    public async Task<ExternalLoginResponseDto> Handle(ExternalLoginCommand request,
        CancellationToken cancellationToken)
    {
        var externalUser = request.Provider.ToLower() switch
        {
            "apple" => await externalAuthService.ValidateAppleTokenAsync(request.IdToken),
            "google" => await externalAuthService.ValidateGoogleTokenAsync(request.IdToken),
            _ => throw new BadRequestException("Unsupported external authentication provider. Supported: Google, Apple")
        };

        if (externalUser == null)
            throw new InvalidCredentialsException("Invalid external authentication token.");

        var (user, isNewUser) = await FindOrCreateExternalUserAsync(externalUser);

        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshTokenResponse = jwtService.GenerateRefreshTokenResponse(user);

        user.SetRefreshToken(refreshTokenResponse.RefreshToken, refreshTokenResponse.RefreshTokenExpiresAt);
        user.UpdateLastLogin();
        await userManager.UpdateAsync(user);

        var externalLoginResponseDto = new ExternalLoginResponseDto
        (
            AccessToken: accessToken.Token,
            RefreshToken: refreshTokenResponse.RefreshToken,
            RefreshTokenExpiration: refreshTokenResponse.RefreshTokenExpiresAt,
            IsNewUser: isNewUser,
            UserId: user.Id.ToString(),
            Email: user.Email!,
            FullName: user.FullName
        );
        return externalLoginResponseDto;
    }

    private async Task<(AppUser User, bool IsNewUser)> FindOrCreateExternalUserAsync(ExternalUserInfo externalUser)
    {
        var user = await userManager.FindByLoginAsync(externalUser.Provider, externalUser.ProviderId);

        if (user is not null)
            return (user, false);

        user = await userManager.FindByEmailAsync(externalUser.Email);

        if (user is not null)
        {
            await userManager.AddLoginAsync(user, new UserLoginInfo(
                externalUser.Provider,
                externalUser.ProviderId,
                externalUser.Provider
            ));
            return (user, false);
        }

        var userName = await GenerateUniqueUserNameAsync(externalUser.Email);
        var fullName = externalUser.FullName ?? externalUser.Email.Split('@')[0];

        user = AppUser.Create(
            userName: userName,
            email: externalUser.Email,
            fullName: fullName
        );

        if (externalUser.EmailVerified)
        {
            user.EmailConfirmed = true;
            user.Activate();
        }

        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
            throw new IdentityException(result.Errors.Select(e => e.Description).ToArray());

        await userManager.AddLoginAsync(user, new UserLoginInfo(
            externalUser.Provider,
            externalUser.ProviderId,
            externalUser.Provider
        ));

        return (user, true);
    }

    private async Task<string> GenerateUniqueUserNameAsync(string email)
    {
        var baseUserName = email.Split('@')[0];
        var userName = baseUserName;
        var suffix = Random.Shared.Next(1000, 9999);

        var attempts = 0;
        while (await userManager.FindByNameAsync(userName) != null && attempts < 10)
        {
            userName = $"{baseUserName}{suffix}";
            suffix = Random.Shared.Next(1000, 9999);
            attempts++;
        }

        if (attempts >= 10)
        {
            userName = $"{baseUserName}{Guid.NewGuid().ToString("N")[..6]}";
        }

        return userName;
    }
}