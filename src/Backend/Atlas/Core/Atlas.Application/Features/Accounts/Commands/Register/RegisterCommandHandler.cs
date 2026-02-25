using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Helpers;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Application.Features.Accounts.Commands.Register;

public class RegisterCommandHandler(
    UserManager<AppUser> userManager,
    IEmailService emailService,
    IApplicationDbContext dbContext,
    IJwtService jwtService
) : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userManager.FindByNameAsync(request.UserName);
        if (existingUser != null)
            throw new AlreadyExistException("User", request.UserName);
        
        var existingEmail = await userManager.FindByEmailAsync(request.Email);
        if (existingEmail != null)
            throw new AlreadyExistException("Email", request.Email);
        
        // Create user with default role (AppUser.Create default is Developer)
        var user = AppUser.Create(
            request.UserName,
            request.Email,
            request.FullName
        );
        
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new IdentityException(result.Errors.Select(e => e.Description).ToArray());
        
        // Assign Identity role based on user's Role property
        await userManager.AddToRoleAsync(user, user.Role.ToString());

        // Create Free subscription for new user
        var subscription = Subscription.CreateFree(user.Id);
        await dbContext.Subscriptions.AddAsync(subscription, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await SendEmailVerificationCodeAsync(user);

        // Generate tokens (reuse Login handler logic)
        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = jwtService.GenerateRefreshTokenResponse(user);
        user.SetRefreshToken(refreshToken.RefreshToken, refreshToken.RefreshTokenExpiresAt);
        user.UpdateLastLogin();
        await userManager.UpdateAsync(user);

        var response = new AuthResponseDto(
            AccessToken: accessToken.Token,
            RefreshToken: refreshToken.RefreshToken,
            AccessTokenExpiration: accessToken.Expiration,
            RefreshTokenExpiration: refreshToken.RefreshTokenExpiresAt,
            UserId: user.Id.ToString(),
            UserName: user.UserName!,
            Email: user.Email!,
            FullName: user.FullName
        );


        return response;
    }

    private async Task SendEmailVerificationCodeAsync(AppUser user)
    {
        var code = VerificationCodeGenerator.Generate();
        user.EmailVerificationCode = code;
        user.EmailVerificationExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);
        await emailService.SendVerificationEmailAsync(user.Email!, code);
    }
}