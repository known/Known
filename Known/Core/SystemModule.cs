namespace Known.Core.Controllers
{
    [Module("系统管理", "fa fa-cogs")]
    public class SystemModule : ModuleBase
    {
        [Page(1, "角色管理", "fa fa-users")]
        public PageView RoleView()
        {
            return new PageView();
        }

        [Page(2, "用户管理", "fa fa-user")]
        public PageView UserView()
        {
            return new PageView();
        }
    }
}
