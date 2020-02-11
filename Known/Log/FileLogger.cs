using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Known.Log
{
    /// <summary>
    /// 文件日志者。
    /// </summary>
    public class FileLogger : Logger, ILogger
    {
        private static readonly Dictionary<long, long> lockDic = new Dictionary<long, long>();
        private readonly string fileName;

        /// <summary>
        /// 初始化一个文件日志者实例，日志路径默认为当前工作目录 logs 文件下。
        /// </summary>
        public FileLogger()
        {
            fileName = Path.Combine(Environment.CurrentDirectory, "logs", DateTime.Now.ToString("yyyyMMdd") + ".log");
        }

        /// <summary>
        /// 初始化一个指定路径的文件日志者实例。
        /// </summary>
        /// <param name="fileName">日志文件路径。</param>
        public FileLogger(string fileName)
        {
            this.fileName = fileName;
        }

        /// <summary>
        /// 输出一行日志内容。
        /// </summary>
        /// <param name="level">日志级别。</param>
        /// <param name="message">日志内容。</param>
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
