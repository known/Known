using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

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
		if (Model.Contents == null || Model.Contents.Count == 0)
			return;

		if (Model.Type == PageType.None)
		{
			foreach (var item in Model.Contents)
			{
				builder.Fragment(item);
			}
		}
		else if (Model.Type == PageType.Column)
		{
            builder.Div($"kui-row-{Model.Spans}", () =>
            {
				foreach(var item in Model.Contents)
				{
                    builder.Div(() => builder.Fragment(item));
                }
            });
        }
		else if (Model.Type == PageType.Row)
		{
			foreach (var item in Model.Contents)
			{
				builder.Div(() => builder.Fragment(item));
			}
		}
	}
}