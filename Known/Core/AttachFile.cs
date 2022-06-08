using System.IO;
using System.Linq;

namespace Known.Core
{
    public interface IAttachFile
    {
        long Length { get; }
        string FileName { get; }

        byte[] GetBytes();
    }

    public class AttachFile
    {
        private readonly IAttachFile file;

        public AttachFile(IAttachFile file, UserInfo user, string typePath = null, string timePath = null)
        {
            this.file = file;
            User = user;
            Size = file.Length;
            var names = file.FileName.Replace(@"\", "/").Split('/');
            SourceName = names.Last();
            ExtName = $".{SourceName.Split('.')[1]}";
            FileName = SourceName;
            var filePath = GetFilePath(user.CompNo, typePath);
            var fileName = $"{user.UserName}_{SourceName}";
            if (string.IsNullOrEmpty(timePath))
                FilePath = $@"{filePath}\{fileName}";
            else
                FilePath = $@"{filePath}\{timePath}\{fileName}";
        }

        internal UserInfo User { get; }
        public long Size { get; }
        public string SourceName { get; }
        public string ExtName { get; }
        public string FileName { get; }
        public string FilePath { get; }
        public string BizId { get; set; }
        public string BizType { get; set; }

        public byte[] GetBytes()
        {
            return file.GetBytes();
        }

        public void Save()
        {
            var filePath = GetUploadPath(FilePath);
            var info = new FileInfo(filePath);
            if (!info.Directory.Exists)
                info.Directory.Create();

            File.WriteAllBytes(filePath, GetBytes());
        }

        public static void Delete(string filePath)
        {
            var path = GetUploadPath(filePath);
            Utils.DeleteFile(path);
        }

        public static byte[] GetBytes(string filePath)
        {
            var path = GetUploadPath(filePath);
            if (!File.Exists(path))
                return null;

            return File.ReadAllBytes(path);
        }

        public static string GetUploadPath(string filePath)
        {
            var app = Config.App;
            var uploadPath = app.UploadPath;
            if (string.IsNullOrEmpty(uploadPath))
            {
                uploadPath = Path.Combine(Config.RootPath, "upload");
            }

            return Path.Combine(uploadPath, filePath);
        }

        private static string GetFilePath(string compNo, string type = null)
        {
            var app = Config.App;
            var filePath = $@"{app.AppId}\{compNo}";

            if (!string.IsNullOrEmpty(type))
            {
                filePath += $@"\{type}";
            }

            return filePath;
        }
    }
}