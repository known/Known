namespace Known.Web
{
    /// <summary>
    /// Api调用结果。
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 返回成功结果。
        /// </summary>
        /// <returns>返回成功状态。</returns>
        public static object Success()
        {
            return new { status = 0 };
        }

        /// <summary>
        /// 返回带数据的成功结果。
        /// </summary>
        /// <typeparam name="T">返回的数据类型。</typeparam>
        /// <param name="data">返回的数据。</param>
        /// <returns>成功结果。</returns>
        public static object Success<T>(T data)
        {
            return new { status = 0, data };
        }

        /// <summary>
        /// 返回错误结果。
        /// </summary>
        /// <param name="message">错误消息。</param>
        /// <returns>错误结果。</returns>
        public static object Error(string message)
        {
            return new { status = 1, message };
        }
    }
}
