using System.Web.SessionState;

namespace Known.Web.Extensions
{
    public static class HttpSessionStateExtension
    {
        public static T GetValue<T>(this HttpSessionState session, string key)
        {
            var value = session[key];
            if (value == null)
                return default(T);

            return (T)value;
        }

        public static void SetValue<T>(this HttpSessionState session, string key, T value)
        {
            session[key] = value;
        }

        public static void ClearValue(this HttpSessionState session, string key)
        {
            session[key] = null;
        }
    }
}
