using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Accounts.Commands.ExternalLogin;

public class ExternalLoginCommandHandler(
    UserManager<AppUser> userManager,
    IJwtService jwtService,
    IExternalAuthService externalAuthService,
    IApplicationDbContext dbContext,
    IEncryptionService encryptionService) 
    : IRequestHandler<ExternalLoginCommand, ExternalLoginResponseDto>
{
    public async Task<ExternalLoginResponseDto> Handle(ExternalLoginCommand request,
        CancellationToken cancellationToken)
    {
        var externalUser = request.Provider.ToLower() switch
        {
            "google" => await externalAuthService.ValidateGoogleTokenAsync(request.IdToken),
            "github" => await externalAuthService.ValidateGitHubTokenAsync(request.AccessToken, request.AuthorizationCode),
            _ => throw new BadRequestException("Unsupported external authentication provider. Supported: Google, GitHub")
        };

        if (externalUser == null)
            throw new InvalidCredentialsException("Invalid external authentication token.");

        var (user, isNewUser) = await FindOrCreateExternalUserAsync(externalUser);

        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshTokenResponse = jwtService.GenerateRefreshTokenResponse(user);

        user.SetRefreshToken(refreshTokenResponse.RefreshToken, refreshTokenResponse.RefreshTokenExpiresAt);
        user.UpdateLastLogin();
        await userManager.UpdateAsync(user);

        var accessTokenForIntegration = externalUser.AccessToken ?? request.AccessToken;

        if (request.Provider.ToLower() == "github" && !string.IsNullOrEmpty(accessTokenForIntegration))
        {
            var existing = await dbContext.Integrations
                .AsQueryable()
                .FirstOrDefaultAsync(i => i.UserProfileId == user.Id && i.Provider == IntegrationProvider.GitHub && !i.IsDeleted, cancellationToken);

            if (existing == null)
            {
                var encryptedToken = encryptionService.Encrypt(accessTokenForIntegration);
                var integration = Integration.Create(
                    userProfileId: user.Id,
                    name: "GitHub",
                    provider: IntegrationProvider.GitHub,
                    encryptedAccessToken: encryptedToken,
                    encryptedRefreshToken: null,
                    expiresAt: null,
                    metadataJson: null
                );

                await dbContext.Integrations.AddAsync(integration, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                var defaultWorkspace = await dbContext.Workspaces.FirstOrDefaultAsync(w => w.UserProfileId == user.Id && w.IsDefault && !w.IsDeleted, cancellationToken);
                if (defaultWorkspace != null)
                {
                    var link = new WorkspaceIntegration
                    {
                        WorkspaceId = defaultWorkspace.Id,
                        IntegrationId = integration.Id,
                        Enabled = true
                    };
                    await dbContext.WorkspaceIntegrations.AddAsync(link, cancellationToken);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }
        }

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