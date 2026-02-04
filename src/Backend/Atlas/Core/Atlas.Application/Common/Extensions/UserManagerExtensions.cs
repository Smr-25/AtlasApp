using Atlas.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Common.Extensions;

public static class UserManagerExtensions
{
    extension(UserManager<AppUser> userManager)
    {
        public async Task<AppUser?> FindByEmailOrUserNameAsync(string? email,
            string? userName)
        {
            if (!string.IsNullOrEmpty(email))
                return await userManager.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (!string.IsNullOrEmpty(userName))
                return await userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            return null;
        }

        public async Task<bool> UserNameExistsAsync(string userName)
        {
            return await userManager.Users.AnyAsync(u => u.UserName == userName);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await userManager.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
        {
            return await userManager.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
        }
    }
}