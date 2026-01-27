namespace Atlas.Application.Common.Helpers;

public static class VerificationCodeGenerator
{
    public static string Generate(int length = 6)
    {
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        var min = (int)Math.Pow(10, length - 1);
        var max = (int)Math.Pow(10, length) - 1;
        return (BitConverter.ToUInt32(bytes, 0) % (max - min + 1) + min).ToString();
    }
}