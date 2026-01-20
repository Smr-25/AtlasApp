using Atlas.Application.Dtos.Users;
using Atlas.Application.Interfaces;
using Atlas.Application.Models;
using Atlas.Application.Services.Interfaces;
using Atlas.Application.Validators.User;
using Atlas.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Application.Services.Concretes;

public class AccountService(
    IApplicationDbContext applicationDbContext,
    IMapper mapper,
    UserManager<AppUser> userManager,
    IValidator<UserRegisterDto> registerValidator,
    IValidator<UserLoginDto> loginValidator,
    JwtService jwtService) : IAccountService
{
    public async Task<ResponseModel<bool>> RegisterAsync(UserRegisterDto userRegisterDto)
    {
        var validationResult = await registerValidator.ValidateAsync(userRegisterDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingUser = await userManager.FindByNameAsync(userRegisterDto.UserName);
        if (existingUser != null)
            return ResponseModel<bool>.Failure("User already exists.");

        var existingEmail = await userManager.FindByEmailAsync(userRegisterDto.Email);
        if (existingEmail != null)
            return ResponseModel<bool>.Failure("Email already in use.");

        var existingPhone = applicationDbContext.Users
            .FirstOrDefault(u => u.PhoneNumber == userRegisterDto.PhoneNumber);
        if (existingPhone != null)
            return ResponseModel<bool>.Failure("Phone number already in use.");

        var user = mapper.Map<AppUser>(userRegisterDto);
        var result = await userManager.CreateAsync(user, userRegisterDto.Password);

        // var code = new Random().Next(100000, 999999).ToString();
        // user.VerificationCode = code;
        // user.VerificationExpiresAt = DateTime.UtcNow.AddMinutes(10);
        // await userManager.UpdateAsync(user);

        return !result.Succeeded
            ? ResponseModel<bool>.Failure(result.Errors.Select(e => e.Description))
            : ResponseModel<bool>.Success(true);
    }

    public async Task<ResponseModel<LoginResponseDto>> LoginAsync(UserLoginDto userLoginDto)
    {
        var validationResult = await loginValidator.ValidateAsync(userLoginDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        var user = await userManager.FindByNameAsync(userLoginDto.UserName);
        if (user == null)
            return ResponseModel<LoginResponseDto>.Failure("Invalid username or password.");
        var passwordValid = await userManager.CheckPasswordAsync(user, userLoginDto.Password);
        if (!passwordValid)
            return ResponseModel<LoginResponseDto>.Failure("Invalid username or password.");
        var token = jwtService.GenerateToken(user);
        var loginResponse = new LoginResponseDto
        {
            Token = token,
            UserName = user.UserName!,
            ExpiresAt = DateTime.UtcNow.AddHours(3)
        };
        return ResponseModel<LoginResponseDto>.Success(loginResponse);
    }
}