namespace FinAlert.StockAlertApi.Models.HttpResponse;

public abstract class ResponseResult
{
    public bool IsSuccess { get; }
    public List<string> Errors { get; }

    protected ResponseResult(bool isSuccess, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? new();
    }

    public static SuccessResult Success() => new SuccessResult();
    public static SuccessResult<T> Success<T>(T value) => new SuccessResult<T>(value);
    public static ErrorResult Failure(List<string> errors) => new ErrorResult(errors);
    public static ErrorResult Failure(string error) => new ErrorResult(new List<string> { error });
}

public class SuccessResult : ResponseResult
{
    public SuccessResult() : base(true) { }
}

public class SuccessResult<T> : ResponseResult
{
    public T Value { get; }

    public SuccessResult(T value) : base(true) => Value = value;
}

public class ErrorResult : ResponseResult
{
    public ErrorResult(List<string> errors) : base(false, errors) { }
}