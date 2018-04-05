namespace Known.Validation
{
    /// <summary>
    /// 验证信息。
    /// </summary>
    public class ValidInfo
    {
        /// <summary>
        /// 构造函数，创建一个验证信息实例。
        /// </summary>
        /// <param name="level">验证级别。</param>
        /// <param name="message">验证消息。</param>
        public ValidInfo(ValidLevel level, string message)
        {
            Level = level;
            Message = message;
        }

        /// <summary>
        /// 取得验证级别。
        /// </summary>
        public ValidLevel Level { get; }

        /// <summary>
        /// 取得验证消息。
        /// </summary>
        public string Message { get; }
    }
}
