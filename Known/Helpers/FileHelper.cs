using System;
using System.IO;

namespace Known.Helpers
{
    /// <summary>
    /// 文件操作帮助者。
    /// </summary>
    public sealed class FileHelper
    {
        /// <summary>
        /// 获取指定文件的扩展名。
        /// </summary>
        /// <param name="fileName">文件路径。</param>
        /// <returns>文件的扩展名。</returns>
        public static string GetFileExtName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            var index = fileName.LastIndexOf('.');
            return fileName.Substring(index).ToLower();
        }

        /// <summary>
        /// 判断是否存在指定的文件。
        /// </summary>
        /// <param name="fileName">文件路径。</param>
        /// <returns>存在返回 True，否则返回 False。</returns>
        public static bool ExistsFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            return File.Exists(fileName);
        }

        /// <summary>
        /// 确定指定文件夹路径存在，不存在则自动创建。
        /// </summary>
        /// <param name="path">文件夹路径。</param>
        /// <returns>文件夹路径。</returns>
        public static string EnsureDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        /// <summary>
        /// 确定指定文件路径存在，不存在则自动创建。
        /// </summary>
        /// <param name="fileName">文件路径。</param>
        /// <returns>文件路径。</returns>
        public static string EnsureFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            return fileName;
        }

        /// <summary>
        /// 删除指定的文件。
        /// </summary>
        /// <param name="fileName">文件路径。</param>
        public static void DeleteFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        /// <summary>
        /// 移动文件，若存在目的文件，则先删除再移动。
        /// </summary>
        /// <param name="sourceFileName">原文件路径。</param>
        /// <param name="destFileName">目的文件路径。</param>
        public static void MoveFile(string sourceFileName, string destFileName)
        {
            EnsureFile(destFileName);
            DeleteFile(destFileName);
            File.Move(sourceFileName, destFileName);
        }

        /// <summary>
        /// 读取文件内容，返回Base64码。
        /// </summary>
        /// <param name="path">文件路径。</param>
        /// <returns>Base64码。</returns>
        public static string ReadBase64String(string path)
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

        /// <summary>
        /// 读取文件内容，返回Base64码。
        /// </summary>
        /// <param name="stream">文件流。</param>
        /// <returns>Base64码。</returns>
        public static string ReadBase64String(Stream stream)
        {
            using (stream)
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return Convert.ToBase64String(bytes);
            }
        }

        /// <summary>
        /// 将Base64码的字符串写入指定路径的文件。
        /// </summary>
        /// <param name="path">文件路径。</param>
        /// <param name="content">Base64码的字符串。</param>
        public static void WriteBase64String(string path, string content)
        {
            var bytes = Convert.FromBase64String(content);
            WriteAllBytes(path, bytes);
        }

        /// <summary>
        /// 将字节写入指定路径的文件。
        /// </summary>
        /// <param name="path">文件路径。</param>
        /// <param name="bytes">字节。</param>
        public static void WriteAllBytes(string path, byte[] bytes)
        {
            if (string.IsNullOrWhiteSpace(path) || bytes == null || bytes.Length == 0)
                return;

            EnsureFile(path);
            File.WriteAllBytes(path, bytes);
        }
    }
}
