namespace Atlas.Application.Common.Exceptions.Common;

public class AlreadyExistException : Exception
{
    public AlreadyExistException(string message)
        : base(message)
    {
    }
    
    public AlreadyExistException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
    
    public AlreadyExistException(string entityName, object key)
        : base($"{entityName} with key '{key}' already exists.")
    {
    }
}