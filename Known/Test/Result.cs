namespace Known
{
    /// <summary>
    /// 验证结果类。
    /// </summary>
    public class Result
    {
        /// <summary>
        /// 初始化一个验证结果类示例。
        /// </summary>
        /// <param name="isValid">是否验证通过。</param>
        /// <param name="message">验证返回的消息。</param>
        /// <param name="data">验证返回的数据。</param>
        protected Result(bool isValid, string message, object data = null)
        {
            IsValid = isValid;
            Message = message;
            Data = data;
        }

        /// <summary>
        /// 取得是否验证通过。
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// 取得验证返回的消息。
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// 取得验证返回的数据。
        /// </summary>
        public object Data { get; }

        /// <summary>
        /// 创建验证失败的结果信息。
        /// </summary>
        /// <param name="message">错误消息。</param>
        /// <param name="data">验证返回的数据。</param>
        /// <returns>结果信息。</returns>
        public static Result Error(string message, object data = null)
        {
            return new Result(false, message, data);
        }

        /// <summary>
        /// 创建验证失败的结果信息，返回泛型类型的数据。
        /// </summary>
        /// <typeparam name="T">返回的数据的泛型。</typeparam>
        /// <param name="message">错误消息。</param>
        /// <param name="data">验证返回的数据。</param>
        /// <returns>结果信息。</returns>
        public static Result<T> Error<T>(string message, T data = default)
        {
            return new Result<T>(false, message, data);
        }

        /// <summary>
        /// 创建验证成功的结果信息。
        /// </summary>
        /// <param name="message">成功消息。</param>
        /// <param name="data">验证返回的数据。</param>
        /// <returns>结果信息。</returns>
        public static Result Success(string message, object data = null)
        {
            return new Result(true, message, data);
        }

        /// <summary>
        /// 创建验证成功的结果信息，返回泛型类型的数据。
        /// </summary>
        /// <typeparam name="T">返回的数据的泛型。</typeparam>
        /// <param name="message">成功消息。</param>
        /// <param name="data">验证返回的数据。</param>
        /// <returns>结果信息。</returns>
        public static Result<T> Success<T>(string message, T data = default)
        {
            return new Result<T>(true, message, data);
        }
    }

    /// <summary>
    /// 验证结果类，返回泛型类型的数据。
    /// </summary>
    /// <typeparam name="T">返回的数据的泛型。</typeparam>
    public class Result<T> : Result
    {
        internal Result(bool isValid, string message, T data)
            : base(isValid, message)
        {
            Data = data;
        }

        /// <summary>
        /// 取得验证返回的数据。
        /// </summary>
        public new T Data { get; }
    }
}
