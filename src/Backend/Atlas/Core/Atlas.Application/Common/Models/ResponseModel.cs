namespace Atlas.Application.Common.Models;

public class ResponseModel<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; set; }
    public IEnumerable<string>? Errors { get; set; }
    
    public static ResponseModel<T> Success(T data)
    {
        return new ResponseModel<T>
        {
            Data = data,
            IsSuccess = true,
            Errors = null
        };
    }
    
    public static ResponseModel<T> Failure(string error)
    {
        return new ResponseModel<T>
        {
            Data = default,
            IsSuccess = false,
            Errors = [error]
        };
    }
    
    public static ResponseModel<T> Failure(IEnumerable<string> errors)
    {
        return new ResponseModel<T>
        {
            Data = default,
            IsSuccess = false,
            Errors = errors
        };
    }
    
}