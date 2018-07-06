namespace Known.Web
{
    public class ApiResult
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }

        public static ApiResult Success()
        {
            return new ApiResult { Status = 0 };
        }

        public static ApiResult Success<T>(T data)
        {
            return new ApiResult { Status = 0, Data = data };
        }

        public static ApiResult Error(string message)
        {
            return new ApiResult { Status = 1, Message = message };
        }
    }
}
