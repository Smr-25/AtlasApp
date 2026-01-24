namespace Atlas.Application.Exceptions.Users;

public class IdentityException : Exception
{
    public IEnumerable<string> Errors { get; }

    public IdentityException(IEnumerable<string> errors)
        : base("One or more identity errors occurred.")
    {
        Errors = errors;
    }

    public IdentityException(string error)
        : base(error)
    {
        Errors = [error];
    }
}