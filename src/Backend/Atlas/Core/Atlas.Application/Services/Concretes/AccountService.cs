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

public class AccountService(IApplicationDbContext applicationDbContext,IMapper mapper,UserManager<AppUser> userManager,IValidator<UserRegisterDto> registerValidator,IValidator<UserLoginDto> loginValidator) : IAccountService
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
        
        var code = new Random().Next(100000, 999999).ToString();
        user.VerificationCode = code;
        user.VerificationExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);
        
        return !result.Succeeded ? ResponseModel<bool>.Failure(result.Errors.Select(e => e.Description)) : ResponseModel<bool>.Success(true);
    }

    public async Task<ResponseModel<bool>> LoginAsync(UserLoginDto userLoginDto)
    {
        var validationResult = await loginValidator.ValidateAsync(userLoginDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        var user = await userManager.FindByNameAsync(userLoginDto.UserName);
        if (user == null)
            return ResponseModel<bool>.Failure("User not found.");
        var passwordValid = await userManager.CheckPasswordAsync(user, userLoginDto.Password);
        return !passwordValid ? ResponseModel<bool>.Failure("Invalid password.") : ResponseModel<bool>.Success(true);
    }
    
}