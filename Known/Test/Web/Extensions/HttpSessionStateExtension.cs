using System.Web.SessionState;

namespace Known.Web.Extensions
{
    /// <summary>
    /// 会话状态扩展类。
    /// </summary>
    public static class HttpSessionStateExtension
    {
        /// <summary>
        /// 获取指定键及类型的会话状态值。
        /// </summary>
        /// <typeparam name="T">值类型。</typeparam>
        /// <param name="session">会话状态对象。</param>
        /// <param name="key">键名。</param>
        /// <returns>会话状态值。</returns>
        public static T GetValue<T>(this HttpSessionState session, string key)
        {
            var value = session[key];
            if (value == null)
                return default;

            return (T)value;
        }

        /// <summary>
        /// 设置指定键及类型的会话状态值。
        /// </summary>
        /// <typeparam name="T">值类型。</typeparam>
        /// <param name="session">会话状态对象。</param>
        /// <param name="key">键名。</param>
        /// <param name="value">会话状态值。</param>
        public static void SetValue<T>(this HttpSessionState session, string key, T value)
        {
            session[key] = value;
        }

        /// <summary>
        /// 清除指定键的会话数据。
        /// </summary>
        /// <param name="session">会话状态对象。</param>
        /// <param name="key">键名。</param>
        public static void ClearValue(this HttpSessionState session, string key)
        {
            session[key] = null;
        }
    }
}
