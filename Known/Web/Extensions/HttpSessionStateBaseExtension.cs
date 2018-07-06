using System.Web;

namespace Known.Web.Extensions
{
    public static class HttpSessionStateBaseExtension
    {
        public static T GetValue<T>(this HttpSessionStateBase session, string key)
        {
            var value = session[key];
            if (value == null)
                return default(T);

            return (T)value;
        }

        public static void SetValue<T>(this HttpSessionStateBase session, string key, T value)
        {
            session[key] = value;
        }

        public static void ClearValue(this HttpSessionStateBase session, string key)
        {
            session[key] = null;
        }
    }
}
