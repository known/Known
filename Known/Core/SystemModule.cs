namespace Known.Core
{
    [Module("系统管理", "fa fa-cogs")]
    public class SystemModule : ModuleBase
    {
        public override string Name => "系统管理";
        public override string Icon => "fa fa-cogs";

        [Page(1, "角色管理", "fa fa-users")]
        public PageView RoleView()
        {
            return Page("Role", "角色管理");
        }

        [Page(2, "用户管理", "fa fa-user")]
        public PageView UserView()
        {
            return Page("User", "用户管理");
        }
    }
}
