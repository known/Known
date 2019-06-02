namespace Known.Web
{
    /// <summary>
    /// Api 操作结果类。
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 取得或设置操作状态，0 表示成功，1 表示失败。
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 取得或设置结果消息。
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 取得或设置操作返回的数据对象。
        /// </summary>
        public dynamic Data { get; set; }

        /// <summary>
        /// 获取操作成功的数据对象。
        /// </summary>
        /// <typeparam name="T">数据对象类型。</typeparam>
        /// <param name="data">数据对象。</param>
        /// <returns>数据对象。</returns>
        public static object ToData<T>(T data)
        {
            return new { ok = true, data };
        }

        /// <summary>
        /// 获取操作成功的分页数据对象。
        /// </summary>
        /// <param name="pr">分页结果对象。</param>
        /// <returns>分页数据对象。</returns>
        public static object ToPageData(PagingResult pr)
        {
            return new { total = pr.TotalCount, data = pr.PageData };
        }

        /// <summary>
        /// 获取操作成功的消息和数据对象。
        /// </summary>
        /// <param name="message">成功提示消息。</param>
        /// <param name="data">数据对象，默认为空。</param>
        /// <returns>消息和数据对象。</returns>
        public static object Success(string message, object data = null)
        {
            return new { ok = true, message, data };
        }

        /// <summary>
        /// 获取操作失败的消息和数据对象。
        /// </summary>
        /// <param name="message">错误提示消息。</param>
        /// <param name="data">数据对象，默认为空。</param>
        /// <returns>消息和数据对象。</returns>
        public static object Error(string message, object data = null)
        {
            return new { ok = false, message, data };
        }

        /// <summary>
        /// 获取操作结果对象。
        /// </summary>
        /// <param name="result">验证结果。</param>
        /// <returns>成功或失败的结果对象。</returns>
        public static object Result(Result result)
        {
            if (!result.IsValid)
                return Error(result.Message);

            return Success(result.Message);
        }

        /// <summary>
        /// 获取指定数据类型的操作结果对象。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="result">验证结果。</param>
        /// <returns>成功或失败的结果对象。</returns>
        public static object Result<T>(Result<T> result)
        {
            if (!result.IsValid)
                return Error(result.Message);

            return Success(result.Message, result.Data);
        }
    }
}
