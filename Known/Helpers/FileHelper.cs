using System;
using System.IO;

namespace Known.Helpers
{
    public sealed class FileHelper
    {
        public static string GetFileBase64String(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            if (!File.Exists(path))
                return string.Empty;

            using (var fs = new FileStream(path, FileMode.Open))
            {
                var bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
