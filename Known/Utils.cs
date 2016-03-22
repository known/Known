using System;

namespace Known
{
    public class Utils
    {
        public static string NewGuid
        {
            get
            {
                return Guid.NewGuid().ToString().ToLower().Replace("-", "");
            }
        }

        public static string RandomNo
        {
            get
            {
                var random = new Random();
                var randomNo = random.Next(1000, 10000).ToString();
                return DateTime.Now.ToString("yyyyMMddHHmmss") + randomNo;
            }
        }
    }
}
