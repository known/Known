using System.Collections.Generic;
using Known.Extensions;
using Known.Mapping;

namespace Known.Platform
{
    public enum ViewType
    {
        None,
        DataGridView,
        TreeGridView,
        TabPageView,
        SplitPageView
    }

    [Table("t_plt_modules", "系统模块")]
    public class Module : BaseEntity
    {
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

        [StringColumn("button_json", "按钮数据")]
        public string ButtonJson { get; set; }

        [StringColumn("field_json", "字段数据")]
        public string FieldJson { get; set; }

        public virtual List<Button> Buttons
        {
            get { return ButtonJson.FromJson<List<Button>>(); }
            set { ButtonJson = value.ToJson(); }
        }

        public virtual List<Field> Fields
        {
            get { return FieldJson.FromJson<List<Field>>(); }
            set { FieldJson = value.ToJson(); }
        }
    }

    #region Button
    public class Button
    {
        public string Parent { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public int Sort { get; set; }
        public bool IsSplit { get; set; }

        public List<Button> Children { get; set; }
    }
    #endregion

    #region Field
    public enum FieldType
    {
        TextBox,
        TextArea,
        Date,
        DateTime,
        Integer,
        Decimal,
        ComboBox,
        CheckBox,
        CheckBoxList,
        RadioButtonList
    }

    public enum QueryType
    {
        None,
        NotEqual,
        Equal,
        LessThan,
        LessEqual,
        GreatThan,
        GreatEqual,
        Between,
        BetweenNotEqual,
        BetweenLessEqual,
        BetweenGreatEqual,
        Contain,
        StartWith,
        EndWith,
        IsNull,
        IsNotNull,
        In,
        NotIn,
        Custom
    }

    public class Field
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public FieldType FieldType { get; set; }
        public QueryType QueryType { get; set; }
        public int Sort { get; set; }
        public bool IsExport { get; set; }
        public bool IsMerge { get; set; }
        public bool IsForm { get; set; }
        public bool IsSort { get; set; }
    }
    #endregion
}
