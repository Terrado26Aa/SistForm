namespace Forms.Models
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public static Result Success() => new Result { IsSuccess = true };
        public static Result Failure(string message) => new Result { IsSuccess = false, ErrorMessage = message };
    }

    public class Result<T> : Result
    {
        public T? Value { get; set; }

        public static Result<T> Success(T value) => new Result<T> { IsSuccess = true, Value = value };
        public new static Result<T> Failure(string message) => new Result<T> { IsSuccess = false, ErrorMessage = message };
    }
}
