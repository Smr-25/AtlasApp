namespace Atlas.Domain.Exceptions;

public abstract class DomainException : Exception
{
    public string ErrorCode { get; }

    protected DomainException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    protected DomainException(string message, string errorCode, Exception innerException) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

public class InvalidEntityStateException : DomainException
{
    public string EntityName { get; }
    public string PropertyName { get; }

    public InvalidEntityStateException(string entityName, string propertyName, string message)
        : base(message, "INVALID_ENTITY_STATE")
    {
        EntityName = entityName;
        PropertyName = propertyName;
    }
}

public class EntityNotFoundException : DomainException
{
    public string EntityName { get; }
    public object EntityId { get; }

    public EntityNotFoundException(string entityName, object entityId)
        : base($"{entityName} with ID '{entityId}' was not found.", "ENTITY_NOT_FOUND")
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}

public class BusinessRuleViolationException : DomainException
{
    public string RuleName { get; }

    public BusinessRuleViolationException(string ruleName, string message)
        : base(message, "BUSINESS_RULE_VIOLATION")
    {
        RuleName = ruleName;
    }
}
