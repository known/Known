namespace Known.Components;

public class TablePicker<TItem> : BasePicker<TItem> where TItem : class, new()
{
    protected TableModel<TItem> Table;

    public override List<TItem> SelectedItems => Table.SelectedRows?.ToList();

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        IsMulti = false;
        Table = new TableModel<TItem>(this)
        {
            IsForm = true,
            AdvSearch = false,
            ShowPager = true,
            SelectType = IsMulti ? TableSelectType.Checkbox : TableSelectType.Radio
        };
    }

    protected override void BuildContent(RenderTreeBuilder builder) => builder.Table(Table);
}

public class UserPicker : TablePicker<SysUser>
{
    private IUserService Service;

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<IUserService>();
        Title = Language["Title.SelectUser"];
        Table.OnQuery = Service.QueryUsersAsync;
        Table.AddColumn(c => c.UserName).Width(100);
        Table.AddColumn(c => c.Name, true).Width(100);
        Table.AddColumn(c => c.Phone).Width(100);
        Table.AddColumn(c => c.Email).Width(100);
        Table.AddColumn(c => c.Role);
    }
}