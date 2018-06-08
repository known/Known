namespace Known.Web
{
    /// <summary>
    /// Api调用结果。
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 取得或设置调用Api的结果状态，0-成功，1-失败。
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 取得或设置调用Api返回的消息。
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 取得或设置调用Api返回的数据。
        /// </summary>
        public dynamic Data { get; set; }

        /// <summary>
        /// 返回成功结果。
        /// </summary>
        /// <returns>返回成功状态。</returns>
        public static ApiResult Success()
        {
            return new ApiResult { Status = 0 };
        }

        /// <summary>
        /// 返回带数据的成功结果。
        /// </summary>
        /// <typeparam name="T">返回的数据类型。</typeparam>
        /// <param name="data">返回的数据。</param>
        /// <returns>成功结果。</returns>
        public static ApiResult Success<T>(T data)
        {
            return new ApiResult { Status = 0, Data = data };
        }

        /// <summary>
        /// 返回错误结果。
        /// </summary>
        /// <param name="message">错误消息。</param>
        /// <returns>错误结果。</returns>
        public static ApiResult Error(string message)
        {
            return new ApiResult { Status = 1, Message = message };
        }
    }
}
