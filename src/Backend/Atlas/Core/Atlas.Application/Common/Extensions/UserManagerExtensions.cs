using Atlas.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Common.Extensions;

public static class UserManagerExtensions
{
    public static async Task<AppUser?> FindByEmailOrUserNameAsync(
        this UserManager<AppUser> userManager,
        string? email,
        string? userName)
    {
        if (!string.IsNullOrEmpty(email))
            return await userManager.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (!string.IsNullOrEmpty(userName))
            return await userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);

        return null;
    }

    public static async Task<bool> UserNameExistsAsync(
        this UserManager<AppUser> userManager,
        string userName)
    {
        return await userManager.Users.AnyAsync(u => u.UserName == userName);
    }

    public static async Task<bool> EmailExistsAsync(
        this UserManager<AppUser> userManager,
        string email)
    {
        return await userManager.Users.AnyAsync(u => u.Email == email);
    }

    public static async Task<bool> PhoneNumberExistsAsync(
        this UserManager<AppUser> userManager,
        string phoneNumber)
    {
        return await userManager.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
    }
}