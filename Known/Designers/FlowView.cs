namespace Known.Designers;

class FlowView : BaseView<FlowInfo>
{
    private TableModel<FlowStepInfo> table;

    internal override async Task SetModelAsync(FlowInfo model)
    {
        await base.SetModelAsync(model);
        await table.RefreshAsync();
    }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Tab.AddTab("Designer.FlowStep", BuildView);

        table = new(this, true)
        {
            FixedHeight = "355px",
            OnQuery = c =>
            {
                var result = new PagingResult<FlowStepInfo>(Model?.Steps);
                return Task.FromResult(result);
            }
        };
        table.Column(c => c.Pass).Template((b, r) => b.Tag(r.Pass));
        table.Column(c => c.Fail).Template((b, r) => b.Tag(r.Fail));
    }

    private void BuildView(RenderTreeBuilder builder)
    {
        builder.Div("bold", $"{Model?.Name}（{Model?.Id}）");
        UI.BuildTable(builder, table);
    }
}