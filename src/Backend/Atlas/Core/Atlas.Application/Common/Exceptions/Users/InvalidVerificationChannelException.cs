namespace Atlas.Application.Common.Exceptions.Users;

public class InvalidVerificationChannelException : Exception
{
    public InvalidVerificationChannelException()
        : base("Invalid verification channel.")
    {
    }

    public InvalidVerificationChannelException(string message)
        : base(message)
    {
    }
}