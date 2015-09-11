using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Known.Models;

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
                return new CodeInfo
                {
                    Category = r.Get<string>("Category"),
                    Code = r.Get<string>("Code"),
                    Name = r.Get<string>("Name"),
                    Sequence = r.Get<int>("Sequence")
                };
            });
            return list;
        }
    }
}
