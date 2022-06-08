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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Known.Entities;

namespace Known.Core
{
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