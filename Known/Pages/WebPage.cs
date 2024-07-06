namespace Known.Pages;

class WebPage : BaseComponent
{
    [Parameter] public PageModel Model { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Model.StateChanged = StateChanged;
    }

	protected override void BuildRender(RenderTreeBuilder builder)
	{
		if (Model.Items == null || Model.Items.Count == 0)
			return;

		if (Model.Type == PageType.None)
		{
			foreach (var item in Model.Items)
			{
				builder.Fragment(item.Content);
			}
		}
		else if (Model.Type == PageType.Column)
		{
            builder.Div($"kui-row-{Model.Spans}", () =>
            {
				foreach(var item in Model.Items)
				{
                    builder.Div(item.ClassName, () => builder.Fragment(item.Content));
                }
            });
        }
		else if (Model.Type == PageType.Row)
		{
			foreach (var item in Model.Items)
			{
				builder.Div(item.ClassName, () => builder.Fragment(item.Content));
			}
		}
	}
}