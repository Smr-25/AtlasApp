namespace Atlas.Application.Exceptions;

public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();

    public ValidationException()
    {
    }

    public ValidationException(string message)
        : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }
    
    public ValidationException(IDictionary<string, string[]> errors)
        : this("One or more validation failures have occurred.")
    {
        Errors = errors;
    }
}