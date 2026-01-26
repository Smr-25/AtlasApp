namespace Atlas.Application.Common.Exceptions.Users;


public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("Invalid username or password.")
    {
    }

    public InvalidCredentialsException(string message)
        : base(message)
    {
    }

    public InvalidCredentialsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
