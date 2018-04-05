using System.Web;

namespace Known.Web.Extensions
{
    /// <summary>
    /// Http会话状态扩展类。
    /// </summary>
    public static class HttpSessionStateBaseExtension
    {
        /// <summary>
        /// 根据会话状态Key获取制定类型的会话状态值。
        /// </summary>
        /// <typeparam name="T">会话状态类型。</typeparam>
        /// <param name="session">Http会话状态。</param>
        /// <param name="key">会话状态Key。</param>
        /// <returns>会话状态值。</returns>
        public static T GetValue<T>(this HttpSessionStateBase session, string key)
        {
            var value = session[key];
            if (value == null)
                return default(T);

            return (T)value;
        }

        /// <summary>
        /// 设置制定类型的会话状态值。
        /// </summary>
        /// <typeparam name="T">会话状态类型。</typeparam>
        /// <param name="session">Http会话状态。</param>
        /// <param name="key">会话状态Key。</param>
        /// <param name="value">会话状态值。</param>
        public static void SetValue<T>(this HttpSessionStateBase session, string key, T value)
        {
            session[key] = value;
        }

        /// <summary>
        /// 根据会话状态Key清除会话状态值。
        /// </summary>
        /// <param name="session">Http会话状态。</param>
        /// <param name="key">会话状态Key。</param>
        public static void ClearValue(this HttpSessionStateBase session, string key)
        {
            session[key] = null;
        }
    }
}
