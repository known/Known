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

namespace Known.Core
{
    partial class SystemService
    {
        [Anonymous]
        public Result PostError(string data)
        {
            var error = Utils.FromJson<LogInfo>(data);
            if (error == null)
                return Result.Error(Language.NotUploadData);

            if (string.IsNullOrEmpty(error.IP))
                error.IP = Context.GetIPAddress();

            error.Id = $"{error.System}{error.User}{error.Message}".GetHashCode().ToString();
            error.CreateTime = DateTime.Now;
            ErrorHelper.AddError(error);
            return Result.Success(Language.UploadSuccess);
        }

        public PagingResult<LogInfo> QueryErrors(PagingCriteria criteria)
        {
            var result = new PagingResult<LogInfo>
            {
                PageData = new List<LogInfo>()
            };

            var datas = ErrorHelper.Errors.OrderByDescending(l => l.CreateTime).ToList();

            if (criteria.HasParameter("System"))
                datas = datas.Where(d => d.System.Contains(criteria.Parameter["System"])).ToList();
            if (criteria.HasParameter("User"))
                datas = datas.Where(d => d.User.Contains(criteria.Parameter["User"])).ToList();

            result.PageData.AddRange(datas);
            result.TotalCount = result.PageData.Count;
            return result;
        }

        public Result DeleteError(string id)
        {
            ErrorHelper.RemoveError(id);
            return Result.Success(Language.DeleteSuccess);
        }
    }
}