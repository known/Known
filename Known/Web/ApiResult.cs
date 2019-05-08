namespace Known.Web
{
    public class ApiResult
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }

        public static object ToData<T>(T data)
        {
            return new { ok = true, data };
        }

        public static object ToPageData(PagingResult pr)
        {
            return new { total = pr.TotalCount, data = pr.PageData };
        }

        public static object Success(string message, object data = null)
        {
            return new { ok = true, message, data };
        }

        public static object Error(string message, object data = null)
        {
            return new { ok = false, message, data };
        }

        public static object Result(Result result)
        {
            if (!result.IsValid)
                return Error(result.Message);

            return Success(result.Message);
        }

        public static object Result<T>(Result<T> result)
        {
            if (!result.IsValid)
                return Error(result.Message);

            return Success(result.Message, result.Data);
        }
    }
}
