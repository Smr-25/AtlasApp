namespace Atlas.Application.Exceptions.Users;

public class AlreadyVerifiedException : Exception
{
    public string VerificationType { get; }

    public AlreadyVerifiedException(string verificationType)
        : base($"{verificationType} is already verified.")
    {
        VerificationType = verificationType;
    }

    public AlreadyVerifiedException(string verificationType, string message)
        : base(message)
    {
        VerificationType = verificationType;
    }
}