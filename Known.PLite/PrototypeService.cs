using Known.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Known.PLite
{
    public interface IPrototypeService
    {
        List<ActionInfo> GetActions();
        List<FieldInfo> GetFields();
    }

    public class PrototypeService : IPrototypeService
    {
        public List<ActionInfo> GetActions()
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select * from T_Actions where IsVisible=1 order by Module,Sequence";
            return command.ToList(r =>
            {
                return new ActionInfo
                {
                    Module = r.Get<string>("Module"),
                    Code = r.Get<string>("Code"),
                    Name = r.Get<string>("Name"),
                    Position = r.Get<ActionPosition>("Position"),
                    Description = r.Get<string>("Description"),
                    Sequence = r.Get<int>("Sequence"),
                    IsVisible = true
                };
            });
        }

        public List<FieldInfo> GetFields()
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select * from T_Fields where IsVisible=1 order by Module,Sequence";
            return command.ToList(r =>
            {
                return new FieldInfo
                {
                    Module = r.Get<string>("Module"),
                    ControlType = r.Get<ControlType>("ControlType"),
                    Code = r.Get<string>("Code"),
                    Name = r.Get<string>("Name"),
                    MaxLength = r.Get<int>("MaxLength"),
                    CodeCategory = r.Get<string>("CodeCategory"),
                    DefaultValue = r.Get<string>("DefaultValue"),
                    IsSearch = r.Get<bool>("IsSearch"),
                    IsList = r.Get<bool>("IsList"),
                    IsEdit = r.Get<bool>("IsEdit"),
                    IsView = r.Get<bool>("IsView"),
                    Sequence = r.Get<int>("Sequence"),
                    IsVisible = true
                };
            });
        }
    }
}
