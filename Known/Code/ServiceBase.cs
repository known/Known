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
using System.Linq;
using Known.Core;

namespace Known
{
    public interface IService { }

    public abstract class ServiceBase
    {
        protected static AppContext Context => AppContext.Current;
        protected static AppInfo App => Config.App;

        internal string PrototypeName { get; set; }

        protected PlatformService Platform
        {
            get
            {
                return new PlatformService
                {
                    CurrentUser = CurrentUser
                };
            }
        }

        private UserInfo currentUser;
        protected UserInfo CurrentUser
        {
            get
            {
                if (currentUser != null)
                    return currentUser;

                return UserHelper.GetUser(out _);
            }
            set
            {
                currentUser = value;
            }
        }

        private Database database;
        public virtual Database Database
        {
            get
            {
                if (database == null)
                    database = new Database();

                database.User = CurrentUser;
                return database;
            }
            set
            {
                database = value;
                Platform.Database = value;
            }
        }

        protected void SetCriteriaAppId(PagingCriteria criteria)
        {
            if (!criteria.Parameter.ContainsKey("AppId"))
                criteria.Parameter["AppId"] = CurrentUser.AppId;
        }

        protected ApiFile GetFileInfo(string path, string fileName)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            return new ApiFile
            {
                FileName = fileName,
                Bytes = AttachFile.GetBytes(path)
            };
        }

        protected Result PostAction<T>(string data, Func<T, Result> func)
        {
            var obj = Utils.FromJson<T>(data);
            if (obj == null)
                return Result.Error(Language.NotPostData);

            return func(obj);
        }

        protected Result ImportAction<T>(string data, Func<List<T>, Result> func) where T : EntityBase
        {
            var datas = Utils.FromJson<List<T>>(data);
            if (datas == null || datas.Count == 0)
                return Result.Error(Language.NotImportData);

            var user = CurrentUser;
            var errors = new List<string>();
            foreach (var item in datas)
            {
                item.CompNo = user.CompNo;
                var vr = item.Validate();
                var error = vr.IsValid ? "" : vr.Message;
                errors.Add(error);
            }

            if (errors.Any(e => !string.IsNullOrEmpty(e)))
                return Result.Error(Language.FailDataCheck, errors);

            return func(datas);
        }
    }
}