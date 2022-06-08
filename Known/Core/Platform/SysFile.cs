using System.Collections.Generic;
using Known.Entities;

namespace Known.Core
{
    partial class PlatformService
    {
        public string AddFile(Database db, AttachFile attach)
        {
            var file = new SysFile
            {
                CompNo = attach.User.CompNo,
                AppId = attach.User.AppId,
                Category1 = "AddFile",
                //Category2 = sysFile.Category2,
                Type = attach.BizType,
                BizId = attach.BizId,
                Name = attach.SourceName,
                Path = attach.FilePath,
                Size = attach.Size,
                SourceName = attach.SourceName,
                ExtName = attach.ExtName
            };
            db.Save(file);
            return file.Id;
        }

        public void DeleteFile(Database db, string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            db.Delete<SysFile>(id);
        }

        public void DeleteFiles(Database db, string bizId, out List<string> filePaths)
        {
            filePaths = new List<string>();
            var files = Repository.GetFiles(Database, bizId);
            if (files == null || files.Count == 0)
                return;

            foreach (var item in files)
            {
                filePaths.Add(item.Path);
                db.Delete(item);
            }
        }

        public void DeleteFiles(List<string> filePaths)
        {
            if (filePaths == null || filePaths.Count == 0)
                return;

            foreach (var item in filePaths)
            {
                AttachFile.Delete(item);
            }
        }
    }

    partial interface IPlatformRepository
    {
        List<SysFile> GetFiles(Database db, string bizId);
    }

    partial class PlatformRepository
    {
        public List<SysFile> GetFiles(Database db, string bizId)
        {
            var sql = "select * from SysFile where BizId=@bizId";
            return db.QueryList<SysFile>(sql, new { bizId });
        }
    }
}
