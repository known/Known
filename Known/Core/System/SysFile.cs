/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Known.Entities;

namespace Known.Core
{
    partial class SystemService
    {
        public List<object> GetFileCategories()
        {
            var user = CurrentUser;
            var lists = new List<object>();
            var cates = SysFileHelper.GetCategories();
            var cates0 = Repository.GetFileCategories(Database, user.AppId, user.CompNo);
            if (cates0 != null && cates0.Count > 0)
            {
                cates.AddRange(cates0);
            }

            var cates1 = cates.Select(c => c.Category1).Distinct();
            foreach (var cate in cates1)
            {
                lists.Add(new { pid = "0", id = cate, name = cate, title = cate, Category1 = cate, Category2 = "" });
                var cates2 = cates.Where(d => d.Category1 == cate)
                                  .Where(d => !string.IsNullOrEmpty(d.Category2))
                                  .Select(d => d.Category2).Distinct();
                foreach (var item in cates2)
                {
                    lists.Add(new
                    {
                        pid = cate,
                        id = $"{cate}_{item}",
                        name = item,
                        title = item,
                        Category1 = cate,
                        Category2 = item
                    });
                }
            }
            return lists;
        }

        public List<SysFile> GetFiles(string bizId)
        {
            return Repository.GetFiles(Database, bizId);
        }

        public PagingResult<SysFile> QueryFiles(PagingCriteria criteria)
        {
            if (criteria.Parameter["Category1"] == SysFileHelper.Category)
            {
                return SysFileHelper.QueryFiles(criteria);
            }

            SetCriteriaAppId(criteria);
            return Repository.QueryFiles(Database, criteria);
        }

        public Result DeleteFiles(string data)
        {
            var ids = Utils.FromJson<string[]>(data);
            var entities = Database.QueryListById<SysFile>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error(Language.SelectOneAtLeast);

            var result = Database.Transaction(Language.Delete, db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                }
            });

            if (result.IsValid)
            {
                var paths = entities.Select(e => e.Path).ToList();
                Platform.DeleteFiles(paths);
            }

            return result;
        }

        public Result SaveFile(string data)
        {
            var model = Utils.ToDynamic(data);
            var entity = Database.QueryById<SysFile>((string)model.Id);
            if (entity == null)
                entity = new SysFile();

            entity.FillModel(model);
            var vr = entity.Validate();
            if (!vr.IsValid)
                return vr;

            Database.Save(entity);
            return Result.Success(Language.SaveSuccess, entity.Id);
        }

        public Result DeleteFile(string id)
        {
            var entity = Database.QueryById<SysFile>(id);
            if (entity == null)
                return Result.Error("附件不存在！");

            Database.Delete(entity);
            AttachFile.Delete(entity.Path);
            return Result.Success(Language.DeleteSuccess);
        }

        public ApiFile DownloadFile(string id)
        {
            var entity = Database.QueryById<SysFile>(id);
            if (entity == null)
                return null;

            return GetFileInfo(entity.Path, entity.SourceName);
        }

        public ApiFile DownloadSysFile(string bizId)
        {
            var entity = SysFileHelper.GetFile(bizId);
            if (entity == null)
                return null;

            return GetFileInfo(entity.Path, entity.SourceName);
        }

        public Result UploadFiles(string data)
        {
            var files = Context.GetFormFiles();
            return UploadHttpFiles(files, data);
        }

        public object Upload()
        {
            var files = Context.GetFormFiles();
            var fileUrls = new List<string>();
            UploadHttpFiles(files, null, fileUrls);
            return new { errno = 0, data = fileUrls };
        }

        private Result UploadHttpFiles(List<IAttachFile> files, string data = null, List<string> fileUrls = null)
        {
            if (files == null || files.Count == 0)
                return Result.Error("附件不能为空！");

            var sysFile = Utils.FromJson<SysFile>(data);
            var user = CurrentUser;
            var sysFiles = new List<SysFile>();
            var time = DateTime.Now.ToString("yyyyMM");
            for (int i = 0; i < files.Count; i++)
            {
                var attach = new AttachFile(files[i], user, null, time);
                attach.Save();

                if (sysFile != null)
                {
                    var entity = new SysFile
                    {
                        AppId = user.AppId,
                        CompNo = user.CompNo,
                        Category1 = sysFile.Category1,
                        Category2 = sysFile.Category2,
                        Type = sysFile.Type,
                        BizId = sysFile.BizId,
                        Name = attach.SourceName,
                        Path = attach.FilePath,
                        Size = attach.Size,
                        SourceName = attach.SourceName,
                        ExtName = attach.ExtName
                    };
                    sysFiles.Add(entity);
                }

                if (fileUrls != null)
                {
                    fileUrls.Add(attach.FilePath.Replace("\\", "/"));
                }
            }

            if (sysFiles.Count > 0)
            {
                Database.Transaction(Language.Save, db =>
                {
                    foreach (var item in sysFiles)
                    {
                        db.Insert(item);
                    }
                });
            }

            return Result.Success(Language.UploadSuccess);
        }

    }

    partial class SystemRepository
    {
        public List<SysFile> GetFileCategories(Database db, string appId, string compNo)
        {
            var sql = "select distinct Category1,Category2 from SysFile where AppId=@appId and CompNo=@compNo";
            return db.QueryList<SysFile>(sql, new { appId, compNo });
        }

        public List<SysFile> GetFiles(Database db, string bizId)
        {
            var sql = "select * from SysFile where BizId=@bizId";
            return db.QueryList<SysFile>(sql, new { bizId });
        }

        public PagingResult<SysFile> QueryFiles(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysFile where AppId=@AppId and CompNo=@CompNo";
            db.SetQuery(ref sql, criteria, QueryType.Equal, "Category1");
            db.SetQuery(ref sql, criteria, QueryType.Equal, "Category2");
            db.SetQuery(ref sql, criteria, QueryType.Contain, "Name");
            db.SetQuery(ref sql, criteria, QueryType.Contain, "SourceName");
            return db.QueryPage<SysFile>(sql, criteria);
        }
    }

    sealed class SysFileHelper
    {
        private static readonly string FilePath = Config.App.Param<string>("SysFilePath");
        internal const string Category = "共用资料";

        private SysFileHelper() { }

        internal static List<SysFile> GetCategories()
        {
            var files = new List<SysFile>();
            if (!Directory.Exists(FilePath))
                return files;

            var dir = new DirectoryInfo(FilePath);
            var folders = dir.GetDirectories();
            if (folders != null && folders.Length > 0)
            {
                files.Add(new SysFile { Category1 = Category });
                foreach (var item in folders)
                {
                    files.Add(new SysFile { Category1 = Category, Category2 = item.Name });
                }
            }

            return files;
        }

        internal static PagingResult<SysFile> QueryFiles(PagingCriteria criteria)
        {
            var files = GetFiles();
            if (criteria.HasParameter("Category2"))
                files = files.Where(f => f.Category2 == criteria.Parameter["Category2"]);
            if (criteria.HasParameter("Name"))
                files = files.Where(f => f.Name.Contains(criteria.Parameter["Name"]));
            if (criteria.HasParameter("ExtName"))
                files = files.Where(f => f.ExtName == criteria.Parameter["Category2"]);

            return new PagingResult<SysFile>
            {
                TotalCount = files.Count(),
                PageData = files.ToList()
            };
        }
        internal static SysFile GetFile(string bizId)
        {
            var files = GetFiles();
            return files.FirstOrDefault(f => f.BizId == bizId);
        }

        private static IEnumerable<SysFile> GetFiles()
        {
            var lists = new List<SysFile>();
            var dir = new DirectoryInfo(FilePath);
            var folders = dir.GetDirectories();
            foreach (var item in folders)
            {
                var files = item.GetFiles();
                foreach (var file in files)
                {
                    lists.Add(new SysFile
                    {
                        Id = "",
                        CreateBy = "系统",
                        Category1 = Category,
                        Category2 = item.Name,
                        Name = file.Name,
                        Type = "",
                        Path = file.FullName,
                        Size = file.Length,
                        SourceName = file.Name,
                        ExtName = file.Extension,
                        BizId = file.FullName.GetHashCode().ToString()
                    });
                }
            }

            return lists;
        }
    }
}