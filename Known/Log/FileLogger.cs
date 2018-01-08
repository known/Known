using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Known.Log
{
    /// <summary>
    /// 文件日志类。
    /// </summary>
    public class FileLogger : Logger, ILogger
    {
        private static Dictionary<long, long> lockDic = new Dictionary<long, long>();
        private string fileName;

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="fileName">日志文件路径。</param>
        public FileLogger(string fileName)
        {
            this.fileName = fileName;
        }

        /// <summary>
        /// 写入单行日志内容。
        /// </summary>
        /// <param name="level">日志级别。</param>
        /// <param name="message">日志内容。</param>
        protected override void WriteLine(LogLevel level, string message)
        {
            if (string.IsNullOrEmpty(fileName))
                return;

            Utils.EnsureFile(fileName);

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
