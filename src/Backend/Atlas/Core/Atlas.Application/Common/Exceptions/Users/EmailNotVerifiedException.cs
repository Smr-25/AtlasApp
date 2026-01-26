namespace Atlas.Application.Common.Exceptions.Users;

public class EmailNotVerifiedException : Exception
{
    public EmailNotVerifiedException()
        : base("Email is not verified.")
    {
    }

    public EmailNotVerifiedException(string message)
        : base(message)
    {
    }
}