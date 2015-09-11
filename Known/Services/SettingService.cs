using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Known.Models;

namespace Known.Services
{
    public interface ISettingService
    {
        List<SettingInfo> GetSettings();
        ValidateResult UpdateSettings(List<SettingInfo> settings);
    }

    public class SettingService : ISettingService
    {
        public List<SettingInfo> GetSettings()
        {
            //var command = DbHelper.Default.CreateCommand();
            //command.Text = "select * from T_SystemSettings";
            //var list = command.ToList(r =>
            //{
            //    return new SettingInfo
            //    {
            //        Code = r.Get<string>("Code"),
            //        Control = r.Get<string>("Control"),
            //        CodeCategory = r.Get<string>("CodeCategory"),
            //        Name = r.Get<string>("Name"),
            //        Value = r.Get<string>("Value")
            //    };
            //});
            //return list;
            return Database.Default.ExecuteReader("select * from T_SystemSettings", null, dr =>
            {
                return new SettingInfo
                {
                    Code = dr.Get<string>("Code"),
                    Control = dr.Get<string>("Control"),
                    CodeCategory = dr.Get<string>("CodeCategory"),
                    Name = dr.Get<string>("Name"),
                    Value = dr.Get<string>("Value")
                };
            }).ToList();
        }

        public ValidateResult UpdateSettings(List<SettingInfo> settings)
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "update T_SystemSettings set Value=?Value where Code=?Code";
            foreach (var item in settings)
            {
                command.Parameters.Add("Code", item.Code);
                command.Parameters.Add("Value", item.Value);
                command.ExecuteOnSubmit();
            }
            var message = DbHelper.Default.SubmitChanges();
            return new ValidateResult(message);
        }
    }
}
