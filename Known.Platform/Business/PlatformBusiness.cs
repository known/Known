using Known.Extensions;

namespace Known.Platform.Business
{
    public abstract class PlatformBusiness : BusinessBase
    {
        public PlatformBusiness(Context context) : base(context)
        {
        }

        public Module GetModule(string id)
        {
            if (id == "demo")
            {
                return new Module
                {
                    Id = "demo",
                    Code = "Demo",
                    Name = "开发示例",
                    ViewType = ViewType.SplitPageView,
                    Extension = new { LeftPartialName = "Demo/DemoMenu" }.ToJson()
                };
            }

            var sql = "select * from t_plt_modules where id=@id";
            return Context.Database.Query<Module>(sql, new { id });
        }
    }
}
