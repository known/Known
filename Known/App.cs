using System.Collections.Generic;

namespace Known
{
    public interface IAppContext
    {
        string GetIPAddress();
        string GetRequestHeader(string key);
        T GetSession<T>(string key);
        void SetSession(string key, object value);
    }

    class AppContext : IAppContext
    {
        private static readonly Dictionary<string, object> session = new Dictionary<string, object>();
        internal static IAppContext Current => Container.Resolve<IAppContext>();

        public string GetIPAddress()
        {
            return string.Empty;
        }

        public string GetRequestHeader(string key)
        {
            return string.Empty;
        }

        public T GetSession<T>(string key)
        {
            if (!session.ContainsKey(key))
                return default;

            return (T)session[key];
        }

        public void SetSession(string key, object value)
        {
            session[key] = value;
        }
    }
}
