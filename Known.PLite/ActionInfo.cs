using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Known.PLite
{
    public class ActionInfo
    {
        public string Id { get; set; }
        public string Module { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public ActionPosition Position { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
        public bool IsVisible { get; set; }

        private static List<ActionInfo> Actions
        {
            get
            {
                var key = KConfig.KeyPrefix + "actioninfos";
                var actions = KCache.Get<List<ActionInfo>>(key);
                if (actions == null)
                {
                    var service = ServiceLoader.Load<IPrototypeService>();
                    actions = service.GetActions();
                    KCache.Insert(key, actions);
                }
                return actions;
            }
        }

        public Control GetControl()
        {
            Control control = null;
            switch (Position)
            {
                case ActionPosition.Toolbar:
                    control = new Button();
                    break;
                case ActionPosition.GridView:
                    control = new LinkButton();
                    break;
            }

            if (control != null)
            {
                control.ID = string.Format("btn{0}", Code);
                (control as IButtonControl).Text = Name;
            }
            return control;
        }

        public static List<ActionInfo> GetActions(string module)
        {
            return Actions.Where(a => a.Module == module).OrderBy(a => a.Sequence).ToList();
        }
    }
}
