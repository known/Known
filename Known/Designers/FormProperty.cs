using Known.Extensions;

namespace Known.Designers;

class FormProperty : BaseProperty
{
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Model.AddRow().AddColumn("行", b => b.Span($"{Column.Row}"))
                      .AddColumn("列", b => b.Span($"{Column.Column}"));
        Model.AddRow().AddColumn("代码类别", b => b.Span(Column.Category));
        Model.AddRow().AddColumn("占位符", b => b.Span($"{Column.Placeholder}"));
        Model.AddRow().AddColumn("单附件", b => b.Span($"{Column.IsFile}"));
        Model.AddRow().AddColumn("多附件", b => b.Span($"{Column.IsMultiFile}"));
        Model.AddRow().AddColumn("必填", b => b.Span($"{Column.IsRequired}"));
        Model.AddRow().AddColumn("只读", b => b.Span($"{Column.IsReadOnly}"));
        Model.AddRow().AddColumn("密码框", b => b.Span($"{Column.IsPassword}"));
        Model.AddRow().AddColumn("选择框", b => b.Span($"{Column.IsSelect}"));
    }
}