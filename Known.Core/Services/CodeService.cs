using Known.Data;
using Known.Extensions;
using System.Collections.Generic;

namespace Known.Services
{
    public interface ICodeService
    {
        List<CodeInfo> GetCodes();
    }

    public class CodeService : ICodeService
    {
        public List<CodeInfo> GetCodes()
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select * from T_CodeTables order by Category,Sequence";
            var list = command.ToList(r =>
            {
                var category = r.Get<string>("Category");
                var code = r.Get<string>("Code");
                var name = r.Get<string>("Name");
                return new CodeInfo(category, code, name);
            });
            return list;
        }
    }
}
