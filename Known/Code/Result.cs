namespace Known
{
    public class Result
    {
        private Result(bool isValid, string message, object data)
        {
            IsValid = isValid;
            Message = message;
            Data = data;
        }

        public bool IsValid { get; }
        public string Message { get; }
        public object Data { get; }

        public static Result Error(string message, object data = null)
        {
            return new Result(false, message, data);
        }

        public static Result Success(string message, object data = null)
        {
            return new Result(true, message, data);
        }
    }
}
