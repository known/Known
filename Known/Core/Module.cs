using System.Collections.Generic;
using Known.Extensions;

namespace Known.Core
{
    public enum ViewType
    {
        None,
        DataGridView,
        TreeGridView,
        TabPageView,
        SplitPageView
    }

    public class Module
    {
        public string Id { get; set; }
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
        public string Extension { get; set; }

        public List<Button> Buttons
        {
            get { return ButtonData.FromJson<List<Button>>(); }
            set { ButtonData = value.ToJson(); }
        }

        public List<Field> Fields
        {
            get { return FieldData.FromJson<List<Field>>(); }
            set { FieldData = value.ToJson(); }
        }

        public Module Parent { get; set; }
        public List<Module> Children { get; set; }

        public List<string> FullCodes
        {
            get
            {
                var fullCodes = new List<string> { this.Code };
                InitFullCodes(this, fullCodes);
                return fullCodes;
            }
        }

        private void InitFullCodes(Module module, List<string> fullCodes)
        {
            if (module.Parent != null)
            {
                fullCodes.Insert(0, module.Parent.Code);
                InitFullCodes(module.Parent, fullCodes);
            }
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
