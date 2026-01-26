namespace Atlas.Application.Common.Exceptions.Users;

public class InvalidOrExpiredCodeException : Exception
{
    public string CodeType { get; }

    public InvalidOrExpiredCodeException(string codeType)
        : base($"Invalid or expired {codeType} code.")
    {
        CodeType = codeType;
    }

    public InvalidOrExpiredCodeException(string codeType, string message)
        : base(message)
    {
        CodeType = codeType;
    }

    public InvalidOrExpiredCodeException(string codeType, string message, Exception innerException)
        : base(message, innerException)
    {
        CodeType = codeType;
    }
}