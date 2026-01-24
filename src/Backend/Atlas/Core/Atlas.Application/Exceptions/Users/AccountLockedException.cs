namespace Atlas.Application.Exceptions.Users;

public class AccountLockedException : Exception
{
    public AccountLockedException(string message) : base(message)
    {
    }
    
    public  AccountLockedException(string message, Exception inner) : base(message, inner)
    {
    }
}