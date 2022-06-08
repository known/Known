using System;
using System.Collections.Generic;
using Known.Entities;

namespace Known.Core
{
    partial class SystemService
    {
        public Result Feedback(List<IAttachFile> httpFiles, string data)
        {
            var user = CurrentUser;
            var filePath = string.Empty;
            var files = new List<ApiFile>();
            if (httpFiles != null && httpFiles.Count > 0)
            {
                var attach = new AttachFile(httpFiles[0], user, "Feedback");
                files.Add(new ApiFile
                {
                    FileName = attach.FileName,
                    Bytes = attach.GetBytes()
                });
                filePath = attach.FilePath;
                attach.Save();
            }

            var d = Utils.FromJson<SysUserMessage>(data);
            var model = new SysUserMessage
            {
                Type = Constants.UMTypeSend,
                MsgBy = "api",
                MsgByName = "系统运维",
                Category = "问题反馈",
                Subject = $"{DateTime.Now:yyyyMMdd}系统问题反馈",
                Content = d.Content,
                FilePath = filePath,
                Status = Constants.UMStatusRead
            };
            return SaveUserMessage(model);
        }
    }
}