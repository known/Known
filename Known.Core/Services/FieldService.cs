using Known.Data;
using Known.Extensions;
using System.Collections.Generic;
using System.Data;

namespace Known.Services
{
    public interface IFieldService
    {
        List<FieldInfo> GetFields(string menu);
        FieldInfo GetField(string menu, string code);
        ValidateResult SaveAs(string menu, FieldInfo field);
    }

    public class FieldService : IFieldService
    {
        public List<FieldInfo> GetFields(string menu)
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select * from T_Fields where Menu=?Menu and IsVisible=1 order by Menu,Sequence";
            command.Parameters.Add("Menu", menu);
            var list = command.ToList(r => { return GetFieldInfo(r); });
            return list;
        }

        public FieldInfo GetField(string menu, string code)
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select * from T_Fields where Menu=?Menu and Code=?Code";
            command.Parameters.Add("Menu", menu);
            command.Parameters.Add("Code", code);
            var entity = command.ToEntity(r => { return GetFieldInfo(r); });
            return entity;
        }

        public ValidateResult SaveAs(string menu, FieldInfo field)
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select count(*) from T_Fields where Menu=?Menu and Code=?Code";
            command.Parameters.Add("Menu", menu);
            command.Parameters.Add("Code", field.Code);
            if (command.ToScalar<int>() > 0)
                command.Text = "update T_Fields set Name=?Name,ControlType=?ControlType,MaxLength=?MaxLength,CodeCategory=?CodeCategory,DefaultValue=?DefaultValue,IsSearch=?IsSearch,IsList=?IsList,IsEdit=?IsEdit,IsView=?IsView,Sequence=?Sequence where Menu=?Menu and Code=?Code";
            else
                command.Text = "insert into T_Fields values(?Menu,?Code,?Name,?ControlType,?MaxLength,?CodeCategory,?DefaultValue,?IsSearch,?IsList,?IsEdit,?IsView,?Sequence,1)";
            command.Parameters.Add("Menu", menu);
            command.Parameters.Add("Code", field.Code);
            command.Parameters.Add("Name", field.Name);
            command.Parameters.Add("ControlType", field.Control);
            command.Parameters.Add("MaxLength", field.MaxLength);
            command.Parameters.Add("CodeCategory", field.CodeCategory);
            command.Parameters.Add("DefaultValue", field.Value);
            command.Parameters.Add("IsSearch", field.IsSearch);
            command.Parameters.Add("IsList", field.IsList);
            command.Parameters.Add("IsEdit", field.IsEdit);
            command.Parameters.Add("IsView", field.IsView);
            command.Parameters.Add("Sequence", field.Sequence);
            var message = command.Execute();
            return new ValidateResult(message);
        }

        private static FieldInfo GetFieldInfo(DataRow row)
        {
            return new FieldInfo
            {
                Code = row.Get<string>("Code"),
                Name = row.Get<string>("Name"),
                Control = row.Get<string>("ControlType"),
                MaxLength = row.Get<int>("MaxLength"),
                CodeCategory = row.Get<string>("CodeCategory"),
                Value = row.Get<string>("DefaultValue"),
                IsSearch = row.Get<bool>("IsSearch"),
                IsList = row.Get<bool>("IsList"),
                IsEdit = row.Get<bool>("IsEdit"),
                IsView = row.Get<bool>("IsView"),
                Sequence = row.Get<int>("Sequence"),
                IsVisible = true
            };
        }
    }
}
