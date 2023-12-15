using Known.Extensions;

namespace Known.Designers;

class PageProperty : BaseProperty
{
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Model.AddRow().AddColumn("默认排序", b => b.Span(Column.DefaultSort));
        Model.AddRow().AddColumn("查看链接", b => b.Span($"{Column.IsViewLink}"));
        Model.AddRow().AddColumn("查询", b => b.Span($"{Column.IsQuery}"));
        Model.AddRow().AddColumn("全部查询", b => b.Span($"{Column.IsQueryAll}"));
    }
}