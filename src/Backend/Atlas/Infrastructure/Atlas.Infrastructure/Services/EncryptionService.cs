using Atlas.Application.Common.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace Atlas.Infrastructure.Services;

public class EncryptionService(IDataProtectionProvider provider) : IEncryptionService
{
    private readonly IDataProtector _protector = provider.CreateProtector("Atlas.TokenProtection.v1");

    public string Encrypt(string input)
    {
        return string.IsNullOrEmpty(input) ? input : _protector.Protect(input);
    }

    public string Decrypt(string input)
    {
        return string.IsNullOrEmpty(input) ? input : _protector.Unprotect(input);
    }
}