namespace Sample.Client.Pages.BizApply;

class ApplyListForm : BaseForm<TbApplyList>
{
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model.SmallLabel = true;
        Model.AddRow().AddColumn(c => c.Item);
        Model.AddRow().AddColumn(c => c.Note, c => c.Type = FieldType.TextArea);
    }
}