namespace Known.Razor;

public class ButtonInfo
{
    public ButtonInfo()
    {
        Enabled = true;
        Visible = true;
        Children = new List<ButtonInfo>();
    }

    public ButtonInfo(string id, string name, string icon, StyleType type = StyleType.Primary) : this()
    {
        Id = id;
        Name = name;
        Icon = icon;
        Type = type;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public StyleType Type { get; set; }
    public bool Enabled { get; set; }
    public bool Visible { get; set; }
    public List<ButtonInfo> Children { get; }

    public bool Is(ButtonInfo info) => Id == info.Id;
}

public sealed class ToolButton
{
    private ToolButton() { }
    static ToolButton()
    {
        Buttons = new List<ButtonInfo>();
        Buttons.AddRange(new List<ButtonInfo> {
            FormButton.Edit,
            New, Copy, DeleteM, Import, Export, ExportPage, ExportQuery, ExportAll,
            Print, Move, MoveUp, MoveDown, Enable, Disable,
            Submit, Revoke, Pass, Return, Repeat,
            ResetPassword, ChangeDepartment
        });
    }

    public static List<ButtonInfo> Buttons { get; }

    public static ButtonInfo New => new("New", "新增", "fa fa-plus");
    public static ButtonInfo Copy => new("Copy", "复制", "fa fa-copy");
    public static ButtonInfo DeleteM => new("DeleteM", "批量删除", "fa fa-trash-o", StyleType.Danger);
    public static ButtonInfo Import => new("Import", "导入", "fa fa-sign-in");
    public static ButtonInfo Export => new("Export", "导出", "fa fa-sign-out");
    public static ButtonInfo ExportPage => new("ExportPage", "导出当前页", "fa fa-sign-out");
    public static ButtonInfo ExportQuery => new("ExportQuery", "导出查询结果", "fa fa-sign-out");
    public static ButtonInfo ExportAll => new("ExportAll", "导出全部", "fa fa-sign-out");
    public static ButtonInfo Print => new("Print", "打印", "fa fa-print");
    public static ButtonInfo Move => new("Move", "移动", "fa fa-arrows");
    public static ButtonInfo MoveUp => new("MoveUp", "上移", "fa fa-arrow-up");
    public static ButtonInfo MoveDown => new("MoveDown", "下移", "fa fa-arrow-down");
    public static ButtonInfo Enable => new("Enable", "启用", "fa fa-check-circle-o");
    public static ButtonInfo Disable => new("Disable", "禁用", "fa fa-times-circle-o", StyleType.Danger);
    public static ButtonInfo Submit => new("Submit", "提交审核", "fa fa-send-o");
    public static ButtonInfo Revoke => new("Revoke", "撤回", "fa fa-undo");
    public static ButtonInfo Assign => new("Assign", "分配给", "fa fa-hand-o-right");
    public static ButtonInfo Pass => new("Pass", "审核通过", "fa fa-check");
    public static ButtonInfo Return => new("Return", "审核退回", "fa fa-reply", StyleType.Danger);
    public static ButtonInfo Repeat => new("Repeat", "重新申请", "fa fa-repeat");
    public static ButtonInfo ResetPassword => new("ResetPassword", "重置密码", "fa fa-rotate-left");
    public static ButtonInfo ChangeDepartment => new("ChangeDepartment", "更换部门", "fa fa-exchange");
}

public sealed class GridAction
{
    private GridAction() { }
    static GridAction()
    {
        Actions = new List<ButtonInfo>();
        Actions.AddRange(new List<ButtonInfo> {
            View, Edit, Delete, Print, MoveUp, MoveDown,
            Verify, Submit, Revoke, Pass, Return, Repeat
        });
    }

    public static List<ButtonInfo> Actions { get; }

    public static ButtonInfo View => new("View", "查看", "fa fa-info-circle");
    public static ButtonInfo Edit => new("Edit", "编辑", "fa fa-edit");
    public static ButtonInfo Delete => new("Delete", "删除", "fa fa-trash-o", StyleType.Danger);
    public static ButtonInfo Print => ToolButton.Print;
    public static ButtonInfo MoveUp => ToolButton.MoveUp;
    public static ButtonInfo MoveDown => ToolButton.MoveDown;
    public static ButtonInfo Verify => new("Verify", "审核", "fa fa-check");
    public static ButtonInfo Submit => ToolButton.Submit;
    public static ButtonInfo Revoke => ToolButton.Revoke;
    public static ButtonInfo Pass => ToolButton.Pass;
    public static ButtonInfo Return => ToolButton.Return;
    public static ButtonInfo Repeat => ToolButton.Repeat;
}

public sealed class FormButton
{
    private FormButton() { }

    public static ButtonInfo Query => new("Query", "查询", "fa fa-search");
    public static ButtonInfo Reset => new("Reset", "重置", "fa fa-undo", StyleType.Default);
    public static ButtonInfo AdvQuery => new("AdvQuery", "高级查询", "");
    public static ButtonInfo Add => new("Add", "添加", "fa fa-plus");
    public static ButtonInfo Save => new("Save", "保存", "fa fa-save");
    public static ButtonInfo Close => new("Close", "关闭", "fa fa-close", StyleType.Danger);
    public static ButtonInfo OK => new("OK", "确定", "fa fa-check");
    public static ButtonInfo Cancel => new("Cancel", "取消", "fa fa-close", StyleType.Danger);
    public static ButtonInfo Back => new("Back", "返回", "fa fa-arrow-left");
    public static ButtonInfo Config => new("Config", "配置", "fa fa-cog", StyleType.Default);
    public static ButtonInfo Edit => new("Edit", "修改", "fa fa-edit");
    public static ButtonInfo EditOK => new("EditOK", "确定修改", "fa fa-check");
}