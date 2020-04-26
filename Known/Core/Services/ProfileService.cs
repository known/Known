using System;
using System.Collections.Generic;
using Known.Core.Entities;

namespace Known.Core.Services
{
    public class ProfileService : ServiceBase
    {
        public Result SaveUserInfo(dynamic model)
        {
            var entity = Database.QueryById<SysUser>((string)model.Id);
            if (entity == null)
                return Result.Error("当前用户不存在！");

            entity.FillModel(model);
            var vr = entity.Validate();
            if (!vr.IsValid)
                return vr;

            Database.Save(entity);
            return Result.Success("保存成功！", entity.Id);
        }

        public Result UpdatePassword(UserInfo user, string oldPassword, string password, string repassword)
        {
            if (user == null)
                return Result.Error("当前用户未登录！");

            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(oldPassword))
                errors.Add("当前密码不能为空！");
            if (string.IsNullOrWhiteSpace(password))
                errors.Add("新密码不能为空！");
            if (string.IsNullOrWhiteSpace(repassword))
                errors.Add("确认新密码不能为空！");
            if (password != repassword)
                errors.Add("两次密码输入不一致！");

            if (errors.Count > 0)
                return Result.Error(string.Join(Environment.NewLine, errors));

            var entity = Database.QueryById<SysUser>(user.Id);
            if (entity == null)
                return Result.Error("当前用户不存在！");

            var pwd = Utils.ToMd5(oldPassword);
            if (entity.Password != pwd)
                return Result.Error("当前密码不正确！");

            entity.Password = Utils.ToMd5(password);
            Database.Save(entity);
            return Result.Success("修改成功！", entity.Id);
        }
    }
}
