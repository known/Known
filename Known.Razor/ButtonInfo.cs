namespace Known.Razor;

public class ButtonInfo
{
    public ButtonInfo() { }
    public ButtonInfo(string id, string name, string icon = null, string style = null)
    {
        Id = id;
        Name = name;
        Icon = icon;
        Style = style;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Style { get; set; }

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
            Print, Move, MoveUp, MoveDown,
            Submit, Revoke, Pass, Return, Repeat
        });
    }

    public static List<ButtonInfo> Buttons { get; }

    public static ButtonInfo New => new("New", "新增", "fa fa-plus", "primary");
    public static ButtonInfo Copy => new("Copy", "复制", "fa fa-copy", "primary");
    public static ButtonInfo DeleteM => new("DeleteM", "批量删除", "fa fa-trash-o", "danger");
    public static ButtonInfo Import => new("Import", "导入", "fa fa-sign-in", "primary");
    public static ButtonInfo Export => new("Export", "导出", "fa fa-sign-out", "primary");
    public static ButtonInfo ExportPage => new("ExportPage", "导出当前页", "fa fa-sign-out", "primary");
    public static ButtonInfo ExportQuery => new("ExportQuery", "导出查询结果", "fa fa-sign-out", "primary");
    public static ButtonInfo ExportAll => new("ExportAll", "导出全部", "fa fa-sign-out", "primary");
    public static ButtonInfo Print => new("Print", "打印", "fa fa-print", "primary");
    public static ButtonInfo Move => new("Move", "移动", "fa fa-arrows", "primary");
    public static ButtonInfo MoveUp => new("MoveUp", "上移", "fa fa-arrow-up", "primary");
    public static ButtonInfo MoveDown => new("MoveDown", "下移", "fa fa-arrow-down", "primary");
    public static ButtonInfo Submit => new("Submit", "提交审核", "fa fa-send-o", "primary");
    public static ButtonInfo Revoke => new("Revoke", "撤回", "fa fa-undo", "primary");
    public static ButtonInfo Pass => new("Pass", "审核通过", "fa fa-check", "primary");
    public static ButtonInfo Return => new("Return", "审核退回", "fa fa-reply", "danger");
    public static ButtonInfo Repeat => new("Repeat", "重新申请", "fa fa-repeat", "primary");
    public static ButtonInfo ResetPassword => new("ResetPassword", "重置密码", "fa fa-rotate-left", "primary");
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

    public static ButtonInfo View => new("View", "查看", "fa fa-info-circle", "primary");
    public static ButtonInfo Edit => new("Edit", "编辑", "fa fa-edit", "primary");
    public static ButtonInfo Delete => new("Delete", "删除", "fa fa-close", "danger");
    public static ButtonInfo Print => ToolButton.Print;
    public static ButtonInfo MoveUp => ToolButton.MoveUp;
    public static ButtonInfo MoveDown => ToolButton.MoveDown;
    public static ButtonInfo Verify => new("Verify", "审核", "fa fa-check", "primary");
    public static ButtonInfo Submit => ToolButton.Submit;
    public static ButtonInfo Revoke => ToolButton.Revoke;
    public static ButtonInfo Pass => ToolButton.Pass;
    public static ButtonInfo Return => ToolButton.Return;
    public static ButtonInfo Repeat => ToolButton.Repeat;
}

public sealed class FormButton
{
    private FormButton() { }

    public static ButtonInfo Query => new("Query", "查询", "fa fa-search", "primary");
    public static ButtonInfo AdvQuery => new("AdvQuery", "高级查询", "", "primary");
    public static ButtonInfo Add => new("Add", "添加", "fa fa-plus", "primary");
    public static ButtonInfo Save => new("Save", "保存", "fa fa-save", "primary");
    public static ButtonInfo Close => new("Close", "关闭", "fa fa-close", "danger");
    public static ButtonInfo OK => new("OK", "确定", "fa fa-check", "primary");
    public static ButtonInfo Cancel => new("Cancel", "取消", "fa fa-close", "danger");
    public static ButtonInfo Back => new("Back", "返回", "fa fa-arrow-left", "primary");
    public static ButtonInfo Config => new("Config", "配置", "fa fa-cog", "primary");
    public static ButtonInfo Edit => new("Edit", "修改", "fa fa-edit", "primary");
    public static ButtonInfo EditOK => new("EditOK", "确定修改", "fa fa-check", "primary");
}