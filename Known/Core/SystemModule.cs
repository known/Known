namespace Known.Core
{
    public class SystemModule : ModuleBase
    {
        public override string Name => "系统管理";
        public override string Icon => "fa fa-cogs";

        public override void Init(AppInfo app, Context context)
        {
        }

        public PageView RoleView()
        {
            return Page(1, "Role", "角色管理", "fa fa-users");
        }

        public PageView UserView()
        {
            return Page(2, "User", "用户管理", "fa fa-user");
        }
    }
}
