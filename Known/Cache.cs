using System.Collections.Generic;

namespace Known
{
    public sealed class Cache
    {
        private const string KEY_CODE = "KN_CODE_INFO";
        private static readonly Dictionary<string, object> cached = new Dictionary<string, object>();

        private Cache() { }

        public static T Get<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return default;

            if (!cached.ContainsKey(key))
                return default;

            return (T)cached[key];
        }

        public static void Set(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            cached[key] = value;
        }

        public static List<CodeInfo> GetCodes()
        {
            var codes = Get<List<CodeInfo>>(KEY_CODE);
            if (codes == null)
                codes = new List<CodeInfo>();

            return codes;
        }

        public static void AddCodes(List<CodeInfo> codes)
        {
            var data = GetCodes();
            data.AddRange(codes);
            Set(KEY_CODE, data);
        }
    }
}
