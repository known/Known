namespace Known
{
    public class Result
    {
        private Result(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }

        public bool IsValid { get; }
        public string Message { get; }
        public dynamic Data { get; private set; }

        public static Result Error(string message, dynamic data = null)
        {
            return new Result(false, message) { Data = data };
        }

        public static Result Success(string message, dynamic data = null)
        {
            return new Result(true, message) { Data = data };
        }
    }
}
