using Known.Mapping;
using Known.Platform;

namespace Known.Core.Entities
{
    public class Module : EntityBase
    {
        public string AppId { get; set; }
        public string ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ViewType ViewType { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public int Sort { get; set; }
        public bool Enabled { get; set; }
        public string ButtonData { get; set; }
        public string FieldData { get; set; }
    }

    class ModuleMapper : EntityMapper<Module>
    {
        public ModuleMapper() :
            base("t_plt_modules", "系统模块")
        {
            this.Property(p => p.AppId)
                .IsStringColumn("app_id", "APPID", 1, 50, true);

            this.Property(p => p.ParentId)
                .IsStringColumn("parent_id", "上级模块ID", 1, 50, true);

            this.Property(p => p.Code)
                .IsStringColumn("code", "编码", 1, 50, true);

            this.Property(p => p.Name)
                .IsStringColumn("name", "名称", 1, 50, true);

            this.Property(p => p.Description)
                .IsStringColumn("description", "描述", 1, 500);

            this.Property(p => p.ViewType)
                .IsEnumColumn("view_type", "视图类型");

            this.Property(p => p.Url)
                .IsStringColumn("url", "地址", 1, 200);

            this.Property(p => p.Icon)
                .IsStringColumn("icon", "图标", 1, 50, true);

            this.Property(p => p.Sort)
                .IsIntegerColumn("sort", "顺序", true);

            this.Property(p => p.Enabled)
                .IsBooleanColumn("enabled", "是否可用");

            this.Property(p => p.ButtonData)
                .IsStringColumn("button_data", "按钮数据");

            this.Property(p => p.FieldData)
                .IsStringColumn("field_data", "字段数据");
        }
    }
}
