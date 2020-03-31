using Known.Core;
using Known.Mapping;

namespace Known.Web.Entities
{
    /// <summary>
    /// 应用程序模块实体类。
    /// </summary>
    public class TModule : EntityBase
    {
        /// <summary>
        /// 取得或设置应用程序ID。
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 取得或设置上级模块ID。
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 取得或设置代码。
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 取得或设置名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置描述。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 取得或设置视图类型。
        /// </summary>
        public ViewType ViewType { get; set; }

        /// <summary>
        /// 取得或设置地址。
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 取得或设置图标。
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 取得或设置顺序。
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 取得或设置是否可用。
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 取得或设置按钮数据。
        /// </summary>
        public string ButtonData { get; set; }

        /// <summary>
        /// 取得或设置字段数据。
        /// </summary>
        public string FieldData { get; set; }
    }

    class TModuleMapper : EntityMapper<TModule>
    {
        public TModuleMapper() :
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
