namespace Atlas.Application.Common.Exceptions.Users;

public class AccountLockedException : Exception
{
    public AccountLockedException(string message) : base(message)
    {
    }
    
    public  AccountLockedException(string message, Exception inner) : base(message, inner)
    {
    }
}