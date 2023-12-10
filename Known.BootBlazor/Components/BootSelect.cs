using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;

namespace Known.BootBlazor.Components;

public class BootSelect : Select<string>
{
	[Parameter] public List<CodeInfo> Codes { get; set; }

	protected override void OnInitialized()
	{
		Items = Codes.ToSelectedItems();
		base.OnInitialized();
	}
}