namespace Known
{
    public class Result
    {
        protected Result(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }

        public bool IsValid { get; }
        public string Message { get; }

        public static Result Error(string message)
        {
            return new Result(false, message);
        }

        public static Result<T> Error<T>(string message, T data = default(T))
        {
            return new Result<T>(false, message, data);
        }

        public static Result Success(string message)
        {
            return new Result(true, message);
        }

        public static Result<T> Success<T>(string message, T data = default(T))
        {
            return new Result<T>(true, message, data);
        }
    }

    public class Result<T> : Result
    {
        internal Result(bool isValid, string message, T data)
            : base(isValid, message)
        {
            Data = data;
        }

        public T Data { get; }
    }
}
