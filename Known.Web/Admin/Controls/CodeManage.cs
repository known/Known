using Known.Services;
using System;
using System.Collections.Generic;

namespace Known.Web.Admin.Controls
{
    public class CodeManage : UserControlBase
    {
        protected List<CodeInfo> Codes = null;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Codes = AppContext.LoadService<ICodeService>().GetCodes();
        }
    }
}
