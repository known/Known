using Known.Mapping;
using Known.Platform;

namespace Known.Core.Entities
{
    [Table("t_plt_modules", "系统模块")]
    public class Module : EntityBase
    {
        [StringColumn("app_id", "APPID", 1, 50, true)]
        public string AppId { get; set; }

        [StringColumn("parent_id", "上级模块ID", 1, 50, true)]
        public string ParentId { get; set; }

        [StringColumn("code", "编码", 1, 50, true)]
        public string Code { get; set; }

        [StringColumn("name", "名称", 1, 50, true)]
        public string Name { get; set; }

        [StringColumn("description", "描述", 1, 500)]
        public string Description { get; set; }

        [EnumColumn("view_type", "视图类型")]
        public ViewType ViewType { get; set; }

        [StringColumn("url", "地址", 1, 200)]
        public string Url { get; set; }

        [StringColumn("icon", "图标", 1, 50, true)]
        public string Icon { get; set; }

        [IntegerColumn("sort", "顺序", true)]
        public int Sort { get; set; }

        [BooleanColumn("enabled", "是否可用")]
        public bool Enabled { get; set; }

        [StringColumn("button_data", "按钮数据")]
        public string ButtonData { get; set; }

        [StringColumn("field_data", "字段数据")]
        public string FieldData { get; set; }
    }
}
