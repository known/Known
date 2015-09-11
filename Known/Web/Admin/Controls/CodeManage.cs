using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Known.Models;

namespace Known.Web.Admin.Controls
{
    public class CodeManage : UserControlBase
    {
        protected List<CodeInfo> Codes = null;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Codes = AppContext.CodeService.GetCodes();
        }
    }
}
