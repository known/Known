using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Known.Log
{
    public class FileLogger : Logger, ILogger
    {
        private static Dictionary<long, long> lockDic = new Dictionary<long, long>();
        private readonly string fileName;

        public FileLogger(string fileName)
        {
            this.fileName = fileName;
        }

        protected override void WriteLine(LogLevel level, string message)
        {
            if (string.IsNullOrEmpty(fileName))
                return;

            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            message += Environment.NewLine;
            using (var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 8, FileOptions.Asynchronous))
            {
                var dataArray = Encoding.UTF8.GetBytes(message);
                bool flag = true;
                long slen = dataArray.Length;
                long len = 0;
                while (flag)
                {
                    try
                    {
                        if (len >= fs.Length)
                        {
                            fs.Lock(len, slen);
                            lockDic[len] = slen;
                            flag = false;
                        }
                        else
                        {
                            len = fs.Length;
                        }
                    }
                    catch
                    {
                        while (!lockDic.ContainsKey(len))
                        {
                            len += lockDic[len];
                        }
                    }
                }
                fs.Seek(len, SeekOrigin.Begin);
                fs.Write(dataArray, 0, dataArray.Length);
                fs.Close();
            }
        }
    }
}
