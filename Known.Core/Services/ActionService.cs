using Known.Data;
using Known.Extensions;
using System.Collections.Generic;
using System.Data;

namespace Known.Services
{
    public interface IActionService
    {
        List<ActionInfo> GetActions(string menu);
        ActionInfo GetAction(string menu, string code);
        ValidateResult SaveAs(string menu, ActionInfo action);
    }

    public class ActionService : IActionService
    {
        public List<ActionInfo> GetActions(string menu)
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select * from T_Actions where Menu=?Menu and IsVisible=1 order by Menu,Sequence";
            command.Parameters.Add("Menu", menu);
            var list = command.ToList(r => { return GetActionInfo(r); });
            return list;
        }

        public ActionInfo GetAction(string menu, string code)
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select * from T_Actions where Menu=?Menu and Code=?Code";
            command.Parameters.Add("Menu", menu);
            command.Parameters.Add("Code", code);
            var entity = command.ToEntity(r => { return GetActionInfo(r); });
            return entity;
        }

        public ValidateResult SaveAs(string menu, ActionInfo action)
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select count(*) from T_Actions where Menu=?Menu and Code=?Code";
            command.Parameters.Add("Menu", menu);
            command.Parameters.Add("Code", action.Code);
            if (command.ToScalar<int>() > 0)
                command.Text = "update T_Actions set Name=?Name,Position=?Position,Description=?Description,Sequence=?Sequence where Menu=?Menu and Code=?Code";
            else
                command.Text = "insert into T_Actions values(?Menu,?Code,?Name,?Position,?Description,?Sequence,1)";
            command.Parameters.Add("Menu", menu);
            command.Parameters.Add("Code", action.Code);
            command.Parameters.Add("Name", action.Name);
            command.Parameters.Add("Position", action.Position);
            command.Parameters.Add("Description", action.Description);
            command.Parameters.Add("Sequence", action.Sequence);
            var message = command.Execute();
            return new ValidateResult(message);
        }

        private static ActionInfo GetActionInfo(DataRow row)
        {
            return new ActionInfo
            {
                Code = row.Get<string>("Code"),
                Name = row.Get<string>("Name"),
                Position = row.Get<string>("Position"),
                Description = row.Get<string>("Description"),
                Sequence = row.Get<int>("Sequence"),
                IsVisible = true
            };
        }
    }
}
