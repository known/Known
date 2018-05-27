namespace Known
{
    /// <summary>
    /// 操作结果类，用于业务逻辑验证返回的结果信息。
    /// </summary>
    public class Result
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="isValid">是否通过验证。</param>
        /// <param name="message">验证结果消息。</param>
        private Result(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }

        /// <summary>
        /// 取得是否通过验证。
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// 取得验证结果消息。
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// 取得返回的数据对象。
        /// </summary>
        public dynamic Data { get; private set; }

        /// <summary>
        /// 创建错误的操作结果实例。
        /// </summary>
        /// <param name="message">错误消息。</param>
        /// <param name="data">返回的错误数据对象。</param>
        /// <returns>操作结果实例。</returns>
        public static Result Error(string message, dynamic data = null)
        {
            return new Result(false, message) { Data = data };
        }

        /// <summary>
        /// 创建成功的操作结果实例。
        /// </summary>
        /// <param name="message">成功消息。</param>
        /// <param name="data">返回的正确数据对象。</param>
        /// <returns>操作结果实例。</returns>
        public static Result Success(string message, dynamic data = null)
        {
            return new Result(true, message) { Data = data };
        }
    }
}
