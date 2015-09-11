using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Known.Models;

namespace Known.Web.Admin.Controls
{
    public class Setting : UserControlBase
    {
        protected List<SettingInfo> Settings = null;
        protected string Keys = string.Empty;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Settings = KSettings.GetSettings();
            Keys = string.Join(",", Settings.Select(s => s.Code).ToArray());
        }
    }
}
