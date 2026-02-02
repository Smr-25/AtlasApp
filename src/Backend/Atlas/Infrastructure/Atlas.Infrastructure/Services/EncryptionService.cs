using System.Text;
using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Services;

public class EncryptionService : IEncryptionService
{
    public string Encrypt(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        var bytes = Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(bytes);
    }

    public string Decrypt(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        var bytes = Convert.FromBase64String(input);
        return Encoding.UTF8.GetString(bytes);
    }
}